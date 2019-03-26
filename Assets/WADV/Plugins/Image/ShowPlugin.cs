using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Plugins.Image.Effects;
using WADV.Plugins.Image.Utilities;
using WADV.Plugins.Unity;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Show")]
    [UsedImplicitly]
    public partial class ShowPlugin : IVisualNovelPlugin {
        private readonly Dictionary<ImageProperties, ImageDisplayInformation> _images = new Dictionary<ImageProperties, ImageDisplayInformation>();
        private TransformValue _defaultTransform = new TransformValue();
        private int _defaultLayer;

        public async Task<SerializableValue> Execute(PluginExecuteContext context) {
            var (mode, layer, effect, images) = AnalyseParameters(context);
            if (!images.Any()) return new NullValue();
            var content = new ImageMessageIntegration.ShowImageContent();
            if (effect == null) {
                content.Effect = null;
            } else {
                if (!(effect.Effect is SingleGraphicEffect singleGraphicEffect)) throw new NotSupportedException($"Unable to create show command: effect {effect} is not SingleGraphicEffect");
                content.Effect = singleGraphicEffect;
            }
            var names = images.Select(e => e.Name).ToArray();
            var preBindImages = FindPreBindImages(names);
            var overlayCanvas = preBindImages.Any() ? await BindImages(preBindImages) : null;
            var imageInformation = InitializeImage(images, layer);
            var targetCanvas = await BindImages(imageInformation);
            RectInt displayArea;
            (overlayCanvas, targetCanvas, displayArea) = CreateDisplayArea(new RectInt(0, 0, targetCanvas.width, targetCanvas.height), overlayCanvas, targetCanvas, mode);
            var overlayName = await PlaceExistedOverlayCanvas(overlayCanvas, displayArea.position, layer);
            await MessageService.ProcessAsync(Message<string[]>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.HideImage, names));
            _images.RemoveAll(e => names.Contains(e.Key.Name));
            await ApplyOverlayEffect(overlayName, targetCanvas, effect?.Effect as SingleGraphicEffect);
            await PlaceTargetImages(images, layer);
            await MessageService.ProcessAsync(Message<string[]>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.HideImage, new[] {overlayName}));
            foreach (var (image, info) in imageInformation) {
                _images.Add(image, info.To(ImageStatus.OnScreen));
            }
            return new NullValue();
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }

        private (ImageBindMode Mode, int Layer, EffectValue Effect, List<ImageProperties> Images) AnalyseParameters(PluginExecuteContext context) {
            EffectValue effect = null;
            var bind = ImageBindMode.None;
            int? layer = null;
            var images = new List<ImageProperties>();
            ImageValue currentImage = null;
            string currentName = null;
            var currentTransform = new TransformValue();
            void AddImage() {
                // ReSharper disable once AccessToModifiedClosure
                var existed = images.FindIndex(e => e.Content.EqualsWith(currentImage, context.Language));
                if (existed > -1) {
                    images.RemoveAt(existed);
                }
                if (string.IsNullOrEmpty(currentName)) throw new MissingMemberException($"Unable to create show command: missing image name for {currentImage.ConvertToString(context.Language)}");
                images.Add(new ImageProperties(currentName, currentImage, (TransformValue) _defaultTransform.AddWith(currentTransform) ?? _defaultTransform));
                currentName = null;
                currentImage = null;
                currentTransform = null;
            }
            foreach (var (key, value) in context.Parameters) {
                switch (key) {
                    case EffectValue effectValue:
                        effect = effectValue;
                        break;
                    case ImageValue imageValue:
                        if (currentImage == null) {
                            currentImage = imageValue;
                            continue;
                        }
                        AddImage();
                        currentImage = imageValue;
                        currentTransform = new TransformValue();
                        break;
                    case IStringConverter stringConverter:
                        var name = stringConverter.ConvertToString(context.Language);
                        switch (name) {
                            case "Layer":
                                layer = IntegerValue.TryParse(value);
                                break;
                            case "Name":
                                currentName = StringValue.TryParse(value);
                                break;
                            case "Effect":
                                effect = value is EffectValue effectValue ? effectValue : throw new ArgumentException($"Unable to create show command: effect {value} is not EffectValue");
                                break;
                            case "Transform":
                                currentTransform.AddWith(value);
                                break;
                            case "Bind":
                                if (value is NullValue) {
                                    bind = ImageBindMode.Canvas;
                                } else {
                                    switch (StringValue.TryParse(value)) {
                                        case "Canvas":
                                        case "Maximal":
                                        case "Max":
                                            bind = ImageBindMode.Canvas;
                                            break;
                                        case "Minimal":
                                        case "Min":
                                            bind = ImageBindMode.Minimal;
                                            break;
                                    }
                                }
                                break;
                            case "DefaultTransform":
                                _defaultTransform = value is TransformValue transformValue ? transformValue : throw new ArgumentException($"Unable to set show command default transform: target {value} is not TransformValue");
                                break;
                            case "DefaultLayer":
                                _defaultLayer = IntegerValue.TryParse(value);
                                break;
                            default:
                                TransformPlugin.AnalyzePropertyTo(name, value, currentTransform, context.Language);
                                break;
                        }
                        break;
                }
            }
            if (currentImage != null) {
                AddImage();
            }
            return (bind, layer ?? _defaultLayer, effect, images);
        }

        private static (Texture2D Overlay, Texture2D Target, RectInt Area) CreateDisplayArea(RectInt displayArea, Texture2D overlay, Texture2D target, ImageBindMode mode) {
            if (mode == ImageBindMode.Minimal) {
                displayArea = overlay == null
                    ? target.GetVisibleContentArea()
                    : overlay.GetVisibleContentArea().MergeWith(target.GetVisibleContentArea());
                if (overlay != null) {
                    overlay = overlay.Cut(displayArea.size);
                }
                target = target.Cut(displayArea.size);
            }
            return (overlay, target, displayArea);
        }

        private static async Task ApplyOverlayEffect(string name, Texture2D target, SingleGraphicEffect effect) {
            var content = new ImageMessageIntegration.ShowImageContent {
                Effect = effect,
                Images = new List<ImageProperties> {new ImageProperties(name, new ImageValue {texture = target}, null)}
            };
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
        }
        
        private static async Task<string> PlaceExistedOverlayCanvas(Texture2D canvas, Vector2Int position, int layer) {
            var name = $"OVERLAY{{{Guid.NewGuid().ToString().ToUpper()}}}";
            var transform = new TransformValue();
            transform.Set(TransformValue.PropertyName.PositionX, position.x);
            transform.Set(TransformValue.PropertyName.PositionY, position.y);
            var content = new ImageMessageIntegration.ShowImageContent {
                Layer = layer,
                Images = new List<ImageProperties> {new ImageProperties(name, new ImageValue {texture = canvas}, transform)}
            };
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
            return name;
        }

        private static async Task PlaceTargetImages(IEnumerable<ImageProperties> images, int layer) {
            var content = new ImageMessageIntegration.ShowImageContent {
                Layer = layer,
                Images = images.ToList()
            };
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
        }
        
        private Dictionary<ImageProperties, ImageDisplayInformation> FindPreBindImages(IEnumerable<string> names) {
            names = names.ToArray();
            var targets = _images.Where(e => names.Contains(e.Key.Name)).ToList();
            var minLayer = targets.Min(e => e.Value.Layer);
            var extraTargets = _images.Where(e => e.Value.Layer >= minLayer).ToList();
            var list = new List<(ImageProperties Image, ImageDisplayInformation DisplayInformation)>();
            foreach (var (key, value) in targets) {
                list.Add((key, value.To(ImageStatus.PrepareToHide)));
            }
            foreach (var (key, value) in extraTargets) {
                list.Add((key, value.To(ImageStatus.OnScreen)));
            }
            list.Sort((x, y) => x.DisplayInformation.Layer - y.DisplayInformation.Layer);
            return list.ToDictionary(e => e.Image, e => e.DisplayInformation);
        }

        private static Dictionary<ImageProperties, ImageDisplayInformation> InitializeImage(IEnumerable<ImageProperties> images, int layer) {
            return images.ToDictionary(image => image, image => new ImageDisplayInformation {Status = ImageStatus.PrepareToShow, Layer = layer});
        }

        private static async Task<Vector2Int> GetImageContainerSize() {
            var result = (await MessageService.ProcessAsync(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetCanvasSize))
                       as Message<Vector2>)?.Content ?? throw new ArgumentException();
            if (result == null) throw new ArgumentException("Unable to create show command: unable to get container size");
            return new Vector2Int((int) Mathf.Ceil(result.x), (int) Mathf.Ceil(result.y));
        }

        private static async Task<Dictionary<ImageProperties, ImageDisplayInformation>> UpdateImages(Dictionary<ImageProperties, ImageDisplayInformation> images) {
            var result = (await MessageService.ProcessAsync(Message<Dictionary<ImageProperties, ImageDisplayInformation>>.Create(
                                                          ImageMessageIntegration.Mask, ImageMessageIntegration.UpdateInformation, images))
                as Message<Dictionary<ImageProperties, ImageDisplayInformation>>)?.Content;
            if (result == null) throw new ArgumentException("Unable to create show command: update image information failed");
            return result;
        }
        
        private static async Task<Texture2D> BindImages(Dictionary<ImageProperties, ImageDisplayInformation> images) {
            images = await UpdateImages(images);
            var canvasSize = await GetImageContainerSize();
            ComputeShader shader = null;
            if (SystemInfo.supportsComputeShaders) {
                shader = (await MessageService.ProcessAsync(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetBindShader))
                    as Message<ComputeShader>)?.Content;
            }
            var canvas = new Texture2DCombiner(canvasSize.x, canvasSize.y, shader);
            foreach (var (image, info) in images) {
                await image.Content.ReadTexture();
                if (image.Content.texture == null) continue;
                if (info.Status == ImageStatus.OnScreen) {
                    canvas.Clear(new RectInt(0, 0, image.Content.texture.width, image.Content.texture.height), info.Transform);
                } else {
                    canvas.DrawTexture(image.Content.texture, info.Transform, image.Content.Color.value);
                }
            }
            return canvas.Combine();
        }
    }
}