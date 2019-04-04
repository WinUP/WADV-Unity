using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Plugins.Image.Effects;
using WADV.Plugins.Image.Utilities;
using WADV.Thread;

namespace WADV.Plugins.Image {
    [RequireComponent(typeof(Canvas))]
    public class ImageCanvas : MonoMessengerBehaviour {
        private readonly Dictionary<string, GameObject> _images = new Dictionary<string, GameObject>();
        private RectTransform _root;

        public ComputeShader computeShader;

        public override int Mask { get; } = ImageMessageIntegration.Mask;
        public override bool IsStandaloneMessage { get; } = false;

        private void Start() {
            _root = GetComponent<RectTransform>();
        }

        public override async Task<Message> Receive(Message message) {
            if (message.HasTag(ImageMessageIntegration.GetCanvasSize))
                return Message<Vector2Int>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetCanvasSize, _root.rect.size.CeilToVector2());
            if (message.HasTag(ImageMessageIntegration.GetBindShader))
                return Message<ComputeShader>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetBindShader, computeShader);
            if (message.HasTag(ImageMessageIntegration.UpdateInformation) && message is Message<ImageDisplayInformation[]> updateMessage) {
                await UpdateImages(updateMessage.Content);
            } else if (message.HasTag(ImageMessageIntegration.ShowImage) && message is Message<ImageMessageIntegration.ShowImageContent> showMessage) {
                await ShowImages(showMessage.Content);
            } else if (message.HasTag(ImageMessageIntegration.HideImage) && message is Message<ImageMessageIntegration.HideImageContent> hideMessage) {
                await HideImages(hideMessage.Content);
            }
            return message;
        }

        private async Task UpdateImages(IReadOnlyList<ImageDisplayInformation> images) {
            var length = images.Count;
            var extraImages = new List<string>();
            for (var i = -1; ++i < length;) {
                var image = images[i];
                switch (image.status) {
                    case ImageStatus.PrepareToHide:
                    case ImageStatus.OnScreen:
                        image.Transform?.ApplyTo(_images[image.Name].GetComponent<RectTransform>());
                        break;
                    default:
                        extraImages.Add(image.Name);
                        CreateImageObject(image).GetComponent<RawImage>().color = Color.clear;
                        break;
                }
            }
            await Dispatcher.NextUpdate();
            for (var i = -1; ++i < length;) {
                var image = images[i];
                image.displayMatrix = GetMatrix(_images[image.Name]);
                if (extraImages.TryRemove(image.Name)) {
                    DestroyImage(image.Name);
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
            var targets = content.Names.Where(e => _images.ContainsKey(e)).ToArray();
            if (content.Effect != null) {
                await content.Effect.PlayEffect(targets.Select(e => _images[e].GetComponent<RawImage>()), null);
            }
            foreach (var target in targets) {
                DestroyImage(target);
            }
        }

        private async Task ShowImages(ImageMessageIntegration.ShowImageContent content) {
            if (content.Effect == null) {
                await Dispatcher.WaitAll(content.Images.Select(e => e.Content.ReadTexture()));
                foreach (var image in content.Images) {
                    if (image.Content.texture == null) continue;
                    if (_images.ContainsKey(image.Name)) {
                        ApplyToImageObject(image, _images[image.Name]);
                    } else {
                        CreateImageObject(image);
                    }
                }
            } else {
                var tasks = new List<Task>();
                var newImages = new List<RawImage>();
                foreach (var image in content.Images) {
                    await image.Content.ReadTexture();
                    if (image.Content.texture == null) continue;
                    if (_images.ContainsKey(image.Name)) {
                        tasks.Add(PlayOnExistedImage(image, content.Effect));
                    } else {
                        newImages.Add(CreateImageObject(image).GetComponent<RawImage>());
                    }
                }
                tasks.Add(content.Effect.PlayEffect(newImages, null));
                await Dispatcher.WaitAll(tasks);
            }
        }

        private void DestroyImage(string target) {
            Destroy(_images[target].gameObject);
            _images.Remove(target);
        }

        private GameObject CreateImageObject(ImageDisplayInformation image) {
            var target = new GameObject(image.Name);
            target.AddComponent<RectTransform>();
            target.AddComponent<RawImage>();
            ApplyToImageObject(image, target);
            _images.Add(image.Name, target);
            return target;
        }

        private void ApplyToImageObject(ImageDisplayInformation image, GameObject target) {
            var targetTransform = target.GetComponent<RectTransform>();
            if (targetTransform.parent != _root) {
                targetTransform.SetParent(_root);
            }
            if (image.Content.texture != null) {
                targetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, image.Content.texture.height);
                targetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, image.Content.texture.width);
                targetTransform.localScale = Vector3.one;
            }
            var position = targetTransform.localPosition;
            targetTransform.localPosition = new Vector3(position.x, position.y, 0);
            image.Transform?.ApplyTo(targetTransform);
            targetTransform.SetSiblingIndex(image.layer);
            var targetImage = target.GetComponent<RawImage>();
            targetImage.texture = image.Content.texture;
            targetImage.uvRect = image.Content.Uv.value;
            targetImage.color = image.Content.Color.value;
        }
        
        private async Task PlayOnExistedImage(ImageDisplayInformation target, SingleGraphicEffect effect) {
            if (effect == null) return;
            var component = _images[target.Name];
            await effect.PlayEffect(new[] {component.GetComponent<RawImage>()}, target.Content.texture);
            ApplyToImageObject(target, component);
        }
    }
}