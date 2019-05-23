using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Plugins.Effect;
using WADV.Plugins.Image.Utilities;
using WADV.Thread;

namespace WADV.Plugins.Image {
    [RequireComponent(typeof(Canvas))]
    public class ImageCanvas : MonoMessengerBehaviour {
        private readonly ImageReferenceList _images = new ImageReferenceList();
        private readonly List<(FrameGraphicEffect Effect, RawImage[] Targets)> _frameGraphicEffects = new List<(FrameGraphicEffect Effect, RawImage[] Targets)>();
        private static Texture2D _defaultTexture;
        private RectTransform _root;

        public ComputeShader computeShader;

        public override int Mask { get; } = ImageMessageIntegration.Mask;
        public override bool IsStandaloneMessage { get; } = false;

        private void Start() {
            _root = GetComponent<RectTransform>();
            if (_defaultTexture == null) {
                _defaultTexture = new Texture2D(1, 1);
                _defaultTexture.SetPixel(0, 0, Color.clear);
                _defaultTexture.Apply(false);
            }
        }

        public override async Task<Message> Receive(Message message) {
            if (message.HasTag(ImageMessageIntegration.GetCanvasSize))
                return Message<Vector2Int>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetCanvasSize, _root.rect.size.CeilToVector2Int());
            if (message.HasTag(ImageMessageIntegration.GetBindShader))
                return computeShader == null
                    ? message
                    : Message<ComputeShader>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetBindShader, computeShader);
            if (message.HasTag(ImageMessageIntegration.UpdateInformation) && message is Message<ImageDisplayInformation[]> updateMessage) {
                await UpdateImages(updateMessage.Content);
            } else if (message.HasTag(ImageMessageIntegration.ShowImage) && message is Message<ImageMessageIntegration.ShowImageContent> showMessage) {
                await ShowImages(showMessage.Content);
            } else if (message.HasTag(ImageMessageIntegration.HideImage) && message is Message<ImageMessageIntegration.HideImageContent> hideMessage) {
                await HideImages(hideMessage.Content);
            } else if (message.HasTag(ImageMessageIntegration.PlayEffect) && message is Message<ImageMessageIntegration.PlayEffectContent> playEffect) {
                await PlayEffect(playEffect.Content);
            }else if (message.HasTag(ImageMessageIntegration.StopEffect) && message is Message<string[]> stopEffect) {
                await StopEffect(stopEffect.Content);
            }
            return message;
        }

        private async Task UpdateImages(ImageDisplayInformation[] images) {
            var length = images.Length;
            var extraImages = new Dictionary<string, GameObject>();
            var readingTasks = new List<Task>();
            for (var i = -1; ++i < length;) {
                var image = images[i];
                switch (image.status) {
                    case ImageStatus.PrepareToHide:
                    case ImageStatus.OnScreen:
                        image.Transform?.ApplyTo(_images.Find(image.Name)?.RenderTarget.GetComponent<RectTransform>());
                        break;
                    default:
                        readingTasks.Add(image.Content.Texture.ReadTexture());
                        var target = CreateImageObject(image, true);
                        target.GetComponent<RawImage>().color = Color.clear;
                        extraImages.Add(image.Name, target);
                        break;
                }
            }
            await Dispatcher.WaitAll(readingTasks);
            await Dispatcher.NextUpdate();
            for (var i = -1; ++i < length;) {
                if (extraImages.ContainsKey(images[i].Name)) {
                    var target = extraImages[images[i].Name];
                    images[i].displayMatrix = GetMatrix(target);
                    Destroy(target);
                } else {
                    images[i].displayMatrix = GetMatrix(_images.Find(images[i].Name)?.RenderTarget);
                }
            }
        }

        private static Matrix4x4 GetMatrix(GameObject target) {
            var image = target.GetComponent<RawImage>().texture;
            var transform = target.GetComponent<RectTransform>();
            var scale = transform.localScale;
            var rect = transform.rect;
            scale.x *= rect.width / image.width;
            scale.y *= rect.height / image.height;
            return Matrix4x4.TRS(transform.anchoredPosition3D, transform.localRotation, scale);
        }

        private async Task HideImages(ImageMessageIntegration.HideImageContent content) {
            var targets = content.Names.Where(e => _images.Contains(e)).ToArray();
            if (content.Effect != null) {
                await content.Effect.PlayEffect(targets.Select(e => _images.Find(e)?.RenderTarget.GetComponent<RawImage>()), null);
            }
            foreach (var target in targets) {
                Destroy(_images.Remove(target)?.RenderTarget);
            }
        }

        private async Task ShowImages(ImageMessageIntegration.ShowImageContent content) {
            await Dispatcher.WaitAll(content.Images.Select(e => e.Content.Texture.ReadTexture()));
            if (content.Effect == null) {
                foreach (var image in content.Images) {
                    if (image.Content.Texture.texture == null) continue;
                    if (_images.Contains(image.Name)) {
                        image.ApplyTo(_images.Find(image.Name)?.RenderTarget, _root);
                    } else {
                        CreateImageObject(image);
                    }
                }
            } else {
                var tasks = new List<Task>();
                foreach (var image in content.Images.Where(e => e.Content.Texture.texture != null)) {
                    tasks.Add(_images.Contains(image.Name)
                        ? PlayOnExistedImage(image, content.Effect)
                        : PlayOnNewImage(image, content.Effect));
                }
                await Dispatcher.WaitAll(tasks);
            }
        }

        private GameObject CreateImageObject(ImageDisplayInformation image, bool isTemp = false) {
            var target = new GameObject(image.Name);
            target.AddComponent<RectTransform>();
            target.AddComponent<RawImage>();
            image.ApplyTo(target, _root);
            var index = isTemp ? _images.Detect(image.layer) : _images.Add(image.Name, target, image.layer);
            target.GetComponent<RectTransform>().SetSiblingIndex(index);
            return target;
        }

        private async Task PlayOnExistedImage(ImageDisplayInformation target, SingleGraphicEffect effect) {
            if (effect == null) return;
            var component = _images.Find(target.Name)?.RenderTarget;
            if (component == null) return;
            var rawImage = component.GetComponent<RawImage>();
            await effect.PlayEffect(new[] {rawImage}, target.Content.Texture.texture);
            target.ApplyTo(component, _root);
        }

        private async Task PlayOnNewImage(ImageDisplayInformation target, SingleGraphicEffect effect) {
            var rawImage = CreateImageObject(target).GetComponent<RawImage>();
            rawImage.texture = _defaultTexture;
            await effect.PlayEffect(new[] {rawImage}, target.Content.Texture.texture);
            rawImage.texture = target.Content.Texture.texture;
            rawImage.material = null;
        }

        private async Task PlayEffect(ImageMessageIntegration.PlayEffectContent content) {
            var tasks = new List<Task>();
            switch (content.Effect) {
                case FrameGraphicEffect frameGraphicEffect:
                    for (var i = -1; ++i < content.Names.Length;) {
                        var target = _images.Find(content.Names[i]);
                        if (target == null) continue;
                        target.Effect = frameGraphicEffect;
                        tasks.Add(frameGraphicEffect.StartEffect(target.RenderTarget.GetComponent<RawImage>()));
                        var index = _frameGraphicEffects.FindIndex(e => e.Effect == frameGraphicEffect);
                        if (index < 0) {
                            _frameGraphicEffects.Add((frameGraphicEffect, new[] {
                                target.RenderTarget.GetComponent<RawImage>()
                            }));
                        } else {
                            var (effect, rawImages) = _frameGraphicEffects[i];
                            Array.Resize(ref rawImages, rawImages.Length + 1);
                            rawImages[rawImages.Length - 1] = target.RenderTarget.GetComponent<RawImage>();
                            _frameGraphicEffects[i] = (effect, rawImages);
                        }
                    }
                    break;
                case StaticGraphicEffect staticGraphicEffect: {
                    for (var i = -1; ++i < content.Names.Length;) {
                        var target = _images.Find(content.Names[i]);
                        if (target == null) continue;
                        target.Effect = staticGraphicEffect;
                        tasks.Add(staticGraphicEffect.StartEffect(target.RenderTarget.GetComponent<RawImage>()));
                    }
                    break;
                }
                case SingleGraphicEffect singleGraphicEffect:
                    var references = new List<RawImage>();
                    for (var i = -1; ++i < content.Names.Length;) {
                        var target = _images.Find(content.Names[i]);
                        if (target == null) continue;
                        target.Effect = singleGraphicEffect;
                        references.Add(target.RenderTarget.GetComponent<RawImage>());
                    }
                    await singleGraphicEffect.PlayEffect(references, null);
                    break;
            }
            await Dispatcher.WaitAll(tasks);
        }

        private async Task StopEffect(string[] images) {
            var tasks = new List<Task>();
            for (var i = -1; ++i < images.Length;) {
                var target = _images.Find(images[i]);
                if (target == null || !(target.Effect is StaticGraphicEffect staticGraphicEffect)) continue;
                var component = target.RenderTarget.GetComponent<RawImage>();
                tasks.Add(staticGraphicEffect.EndEffect(component));
                if (!(staticGraphicEffect is FrameGraphicEffect frameGraphicEffect)) continue;
                var index = _frameGraphicEffects.FindIndex(e => e.Effect == frameGraphicEffect);
                if (index < 0) continue;
                var (_, targets) = _frameGraphicEffects[index];
                if (targets.Length <= 1) {
                    _frameGraphicEffects.RemoveAt(index);
                } else {
                    targets = targets.Where(e => e != component).ToArray();
                }
                _frameGraphicEffects[index] = (frameGraphicEffect, targets);
            }
            await Dispatcher.WaitAll(tasks);
        }
        
        public void Update() {
            for (var i = -1; ++i < _frameGraphicEffects.Count;) {
                var (effect, targets) = _frameGraphicEffects[i];
                for (var j = -1; ++j < targets.Length;) {
                    effect.UpdateEffect(targets[j]);
                }
            }
        }
    }
}