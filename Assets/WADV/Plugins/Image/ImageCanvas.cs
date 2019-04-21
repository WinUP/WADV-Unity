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
        private readonly ImageList _images = new ImageList();
        private RectTransform _root;

        public ComputeShader computeShader;

        public override int Mask { get; } = ImageMessageIntegration.Mask;
        public override bool IsStandaloneMessage { get; } = false;

        private void Start() {
            _root = GetComponent<RectTransform>();
        }

        public override async Task<Message> Receive(Message message) {
            if (message.HasTag(ImageMessageIntegration.GetCanvasSize))
                return Message<Vector2Int>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetCanvasSize, _root.rect.size.CeilToVector2Int());
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

        private async Task UpdateImages(ImageDisplayInformation[] images) {
            var length = images.Length;
            var extraImages = new List<string>();
            for (var i = -1; ++i < length;) {
                var image = images[i];
                switch (image.status) {
                    case ImageStatus.PrepareToHide:
                    case ImageStatus.OnScreen:
                        image.Transform?.ApplyTo(_images.Find(image.Name).GetComponent<RectTransform>());
                        break;
                    default:
                        extraImages.Add(image.Name);
                        CreateImageObject(image).GetComponent<RawImage>().color = Color.clear;
                        break;
                }
            }
            await Dispatcher.NextUpdate();
            for (var i = -1; ++i < length;) {
                images[i].displayMatrix = GetMatrix(_images.Find(images[i].Name));
                if (extraImages.TryRemove(images[i].Name)) {
                    DestroyImage(images[i].Name);
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
                DestroyImage(target);
            }
        }

        private async Task ShowImages(ImageMessageIntegration.ShowImageContent content) {
            if (content.Effect == null) {
                await Dispatcher.WaitAll(content.Images.Select(e => e.Content.Texture.ReadTexture()));
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
                var newImages = new List<RawImage>();
                foreach (var image in content.Images) {
                    await image.Content.Texture.ReadTexture();
                    if (image.Content.Texture.texture == null) continue;
                    if (_images.Contains(image.Name)) {
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
            _images.Destroy(target);
        }

        private GameObject CreateImageObject(ImageDisplayInformation image) {
            var target = new GameObject(image.Name);
            target.AddComponent<RectTransform>();
            target.AddComponent<RawImage>();
            image.ApplyTo(target, _root);
            target.GetComponent<RectTransform>().SetSiblingIndex(_images.Add(image.Name, target, image.layer));
            return target;
        }

        private async Task PlayOnExistedImage(ImageDisplayInformation target, SingleGraphicEffect effect) {
            if (effect == null) return;
            var component = _images.Find(target.Name);
            await effect.PlayEffect(new[] {component.GetComponent<RawImage>()}, target.Content.Texture.texture);
            target.ApplyTo(component, _root);
        }
        
        private class ImageList {
            private readonly Dictionary<string, GameObject> _imageIndex = new Dictionary<string, GameObject>();
            private readonly List<(string Name, int Sibling)> _images = new List<(string, int)>();

            public int Add(string name, GameObject target, int layer) {
                if (_imageIndex.ContainsKey(name)) {
                    Destroy(name);
                }
                _imageIndex.Add(name, target);
                var result = (name, layer);
                if (_images.Count == 0) {
                    _images.Add(result);
                    return 0;
                }
                // 鉴于大多数情况下图片数量最多也就几十张，为二分查找开空间不值得，可直接使用线性查找
                var index = _images.FindIndex(e => e.Sibling > layer);
                if (index < 0) {
                    _images.Add(result);
                    return _images.Count - 1;
                }
                _images.Insert(index, result);
                return index;
            }
            
            public GameObject Find(string name) {
                return _imageIndex.TryGetValue(name, out var target) ? target : null;
            }

            public bool Contains(string name) {
                return _imageIndex.ContainsKey(name);
            }

            public void Destroy(string name) {
                if (!_imageIndex.TryGetValue(name, out var target)) return;
                Object.Destroy(target);
                _images.RemoveAt(_images.FindIndex(e => e.Name == name));
                _imageIndex.Remove(name);
            }
        }
    }
}