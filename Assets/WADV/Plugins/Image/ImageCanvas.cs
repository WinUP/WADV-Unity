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
        private readonly Dictionary<string, ImageProperties> _properties = new Dictionary<string, ImageProperties>();
        private RectTransform _root;

        public override int Mask { get; } = ImageMessageIntegration.Mask;
        public override bool IsStandaloneMessage { get; } = false;

        private void Start() {
            _root = GetComponent<RectTransform>();
        }

        public override async Task<Message> Receive(Message message) {
            if (message.HasTag(ImageMessageIntegration.ShowImage) && message is Message<ImageMessageIntegration.ShowImageContent> showMessage) {
                await ShowImages(showMessage.Content);
            } else if (message.HasTag(ImageMessageIntegration.HideImage) && message is Message<string[]> hideMessage) {
                await HideImages(hideMessage.Content);
            }
            return message;
        }

        private async Task HideImages(string[] names) {
            
        }

        private async Task ShowImages(ImageMessageIntegration.ShowImageContent content) {
            async Task PlayOnExistedImage(ImageProperties target) {
                if (content.Effect == null) return;
                var component = _images[target.Name];
                await content.Effect.PlayEffect(new[] {component}, target.Content.texture);
                PrepareImage(target, content.Layer, component.gameObject);
            }
            if (content.Effect == null) {
                var names = content.Images.Select(e => e.Name).ToArray();
                _images.RemoveAll(names);
                _properties.RemoveAll(names);
                foreach (var image in content.Images) {
                    await image.Content.ReadTexture();
                    if (image.Content.texture == null) continue;
                    PrepareImage(image, content.Layer);
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
                        newImages.Add(PrepareImage(image, content.Layer).Image);
                    }
                }
                tasks.Add(content.Effect.PlayEffect(newImages, null));
                await Dispatcher.WaitAll(tasks);
            }
        }

        private void DestroyImage(string target) {
            Destroy(_images[target].gameObject);
            _images.Remove(target);
            _properties.Remove(target);
        }

        private (RectTransform Transform, RawImage Image) PrepareImage(ImageProperties image, int layer, GameObject target = null) {
            if (target == null) {
                target = new GameObject();
                target.AddComponent<RectTransform>();
            }
            var targetTransform = target.GetComponent<RectTransform>();
            image.Transform?.ApplyTo(targetTransform);
            targetTransform.SetSiblingIndex(layer);
            var targetImage = target.AddComponent<RawImage>();
            targetImage.texture = image.Content.texture;
            targetImage.uvRect = image.Content.Uv.value;
            targetImage.color = image.Content.Color.value;
            targetTransform.parent = _root;
            if (_images.ContainsKey(image.Name)) {
                DestroyImage(image.Name);
            }
            _images.Add(image.Name, targetImage);
            _properties.Add(image.Name, image);
            return (targetTransform, targetImage);
        }
    }
}