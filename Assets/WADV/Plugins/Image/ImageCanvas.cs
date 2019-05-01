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
        private readonly ImageSiblingList _images = new ImageSiblingList();
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
                        image.Transform?.ApplyTo(_images.Find(image.Name).GetComponent<RectTransform>());
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
                    images[i].displayMatrix = GetMatrix(_images.Find(images[i].Name));
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
                await content.Effect.PlayEffect(targets.Select(e => _images.Find(e).GetComponent<RawImage>()), null);
            }
            foreach (var target in targets) {
                Destroy(_images.Remove(target));
            }
        }

        private async Task ShowImages(ImageMessageIntegration.ShowImageContent content) {
            await Dispatcher.WaitAll(content.Images.Select(e => e.Content.Texture.ReadTexture()));
            if (content.Effect == null) {
                foreach (var image in content.Images) {
                    if (image.Content.Texture.texture == null) continue;
                    if (_images.Contains(image.Name)) {
                        image.ApplyTo(_images.Find(image.Name), _root);
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
            var component = _images.Find(target.Name);
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
    }
}