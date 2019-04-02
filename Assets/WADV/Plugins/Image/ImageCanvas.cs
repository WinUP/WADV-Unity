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
        private readonly Dictionary<string, RawImage> _images = new Dictionary<string, RawImage>();
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
                        image.Transform?.ApplyTo(_images[image.Name].rectTransform);
                        break;
                    default:
                        extraImages.Add(image.Name);
                        PrepareImage(image).Image.color = Color.clear;
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

        private static Matrix4x4 GetMatrix(RawImage target) {
            var image = target.texture;
            var transform = target.GetComponent<RectTransform>();
            var scale = transform.localScale;
            var rect = transform.rect;
            scale.x *= rect.width / image.width;
            scale.y *= rect.height / image.height;
            return Matrix4x4.TRS(transform.anchoredPosition3D, transform.localRotation, scale);
        }

        private async Task HideImages(ImageMessageIntegration.HideImageContent content) {
            if (content.Effect == null) {
                
            }
        }

        private async Task ShowImages(ImageMessageIntegration.ShowImageContent content) {
            async Task PlayOnExistedImage(ImageDisplayInformation target) {
                if (content.Effect == null) return;
                var component = _images[target.Name];
                await content.Effect.PlayEffect(new[] {component}, target.Content.texture);
                PrepareImage(target, component.gameObject);
            }
            if (content.Effect == null) {
                foreach (var image in content.Images) {
                    await image.Content.ReadTexture();
                    if (image.Content.texture == null) continue;
                    if (_images.ContainsKey(image.Name)) {
                        PrepareImage(image, _images[image.Name].gameObject);
                    } else {
                        PrepareImage(image);
                    }
                }
            } else {
                var tasks = new List<Task>();
                var newImages = new List<RawImage>();
                foreach (var image in content.Images) {
                    await image.Content.ReadTexture();
                    if (image.Content.texture == null) continue;
                    if (_images.ContainsKey(image.Name)) {
                        tasks.Add(PlayOnExistedImage(image));
                    } else {
                        newImages.Add(PrepareImage(image).Image);
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

        private (RectTransform Transform, RawImage Image) PrepareImage(ImageDisplayInformation image, GameObject target = null) {
            target = target ? target : new GameObject();
            var targetTransform = target.GetComponent<RectTransform>() ?? target.AddComponent<RectTransform>();
            image.Transform?.ApplyTo(targetTransform);
            targetTransform.SetSiblingIndex(image.layer);
            var targetImage = target.GetComponent<RawImage>() ?? target.AddComponent<RawImage>();
            targetImage.texture = image.Content.texture;
            targetImage.uvRect = image.Content.Uv.value;
            targetImage.color = image.Content.Color.value;
            targetTransform.parent = _root;
            if (_images.ContainsKey(image.Name)) {
                DestroyImage(image.Name);
            }
            _images.Add(image.Name, targetImage);
            return (targetTransform, targetImage);
        }
    }
}