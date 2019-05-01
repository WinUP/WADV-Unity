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
using WADV.Thread;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Show")]
    [UsedImplicitly]
    public partial class ShowPlugin : IVisualNovelPlugin {
        private Dictionary<string, ImageDisplayInformation> _images = new Dictionary<string, ImageDisplayInformation>();
        private TransformValue _defaultTransform = new TransformValue();
        private int _defaultLayer;
        private MainThreadPlaceholder _placeholder;

        public ShowPlugin() {
            _defaultTransform.Set(TransformValue.PropertyName.PositionX, 0);
            _defaultTransform.Set(TransformValue.PropertyName.PositionY, 0);
            _defaultTransform.Set(TransformValue.PropertyName.PositionZ, 0);
            _defaultTransform.Set(TransformValue.PropertyName.AnchorMinX, 0);
            _defaultTransform.Set(TransformValue.PropertyName.AnchorMinY, 0);
            _defaultTransform.Set(TransformValue.PropertyName.AnchorMaxX, 0);
            _defaultTransform.Set(TransformValue.PropertyName.AnchorMaxY, 0);
            _defaultTransform.Set(TransformValue.PropertyName.PivotX, 0);
            _defaultTransform.Set(TransformValue.PropertyName.PivotY, 0);
        }

        public async Task<SerializableValue> Execute(PluginExecuteContext context) {
            var (mode, layer, effect, images) = AnalyseParameters(context);
            if (!images.Any()) return new NullValue();
            CreatePlaceholder();
            InitializeImage(images, layer);
            if (effect == null) {
                await PlaceNewImages(images);
                UpdateImageInfo(images);
                CompletePlaceholder();
                return new NullValue();
            }
            if (mode == ImageBindMode.None) {
                // TODO: Images separate display
                CompletePlaceholder();
                return new NullValue();
            }
            var names = images.Select(e => e.Name).ToArray();
            var overlayCanvas = await BindImages(FindPreBindImages(names));
            var targetCanvas = await BindImages(images);
            RectInt displayArea;
            (overlayCanvas, targetCanvas, displayArea) = CutDisplayArea(new RectInt(0, 0, targetCanvas.width, targetCanvas.height), overlayCanvas, targetCanvas, mode);
            var overlayName = $"OVERLAY{{{Guid.NewGuid().ToString().ToUpper()}}}";
            var overlayTransform = CreateOverlayTransform(displayArea.position);
            await PlaceOverlayCanvas(overlayName, overlayCanvas, overlayTransform, layer);
            await RemoveHiddenSeparateImages(names);
            await PlayOverlayEffect(overlayName, targetCanvas, effect.Effect as SingleGraphicEffect, overlayTransform, layer);
            await PlaceNewImages(images);
            await RemoveOverlayImage(overlayName);
            UpdateImageInfo(images);
            CompletePlaceholder();
            return new NullValue();
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }

        private void CreatePlaceholder() {
            if (_placeholder != null) {
                CompletePlaceholder();
            }
            _placeholder = Dispatcher.CreatePlaceholder();
        }

        private void CompletePlaceholder() {
            _placeholder.Complete();
            _placeholder = null;
        }

        private void AddImage(ref ImageValue currentImage, ref string currentName, ref TransformValue currentTransform, List<ImageDisplayInformation> images, PluginExecuteContext context) {
            if (string.IsNullOrEmpty(currentName)) throw new MissingMemberException($"Unable to create show command: missing image name for {currentImage.ConvertToString(context.Language)}");
            // ReSharper disable once AccessToModifiedClosure
            var name = currentName;
            var image = currentImage;
            images.RemoveAll(e => e.Name == name || e.Content.EqualsWith(image, context.Language));
            images.Add(new ImageDisplayInformation(currentName, currentImage, currentTransform == null ? null : (TransformValue) _defaultTransform.AddWith(currentTransform)));
            currentName = null;
            currentImage = null;
            currentTransform = null;
        }
        
        private (ImageBindMode Mode, int Layer, EffectValue Effect, ImageDisplayInformation[] Images) AnalyseParameters(PluginExecuteContext context) {
            EffectValue effect = null;
            var bind = ImageBindMode.None;
            int? layer = null;
            var images = new List<ImageDisplayInformation>();
            ImageValue currentImage = null;
            string currentName = null;
            TransformValue currentTransform = null;
            foreach (var (key, value) in context.Parameters) {
                switch (key) {
                    case EffectValue effectValue:
                        effect = effectValue;
                        break;
                    case ImageValue imageValue:
                        if (currentImage != null) {
                            AddImage(ref currentImage, ref currentName, ref currentTransform, images, context);
                        }
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
                                currentTransform = (TransformValue) currentTransform.AddWith(value);
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
                AddImage(ref currentImage, ref currentName, ref currentTransform, images, context);
            }
            layer = layer ?? _defaultLayer;
            var list = images.ToArray();
            for (var i = -1; ++i < list.Length;) {
                list[i].layer = layer.Value;
                list[i].status = ImageStatus.PrepareToShow;
            }
            return (bind, layer.Value, effect, list);
        }

        private ImageDisplayInformation[] FindPreBindImages(string[] names) {
            if (!_images.Any()) return new ImageDisplayInformation[] { };
            var layers = _images.Where(e => names.Contains(e.Key)).Select(e => e.Value.layer).ToArray();
            if (!layers.Any()) return new ImageDisplayInformation[] { };
            var minLayer = layers.Min();
            var targets = _images.Where(e => e.Value.layer >= minLayer).Select(e => e.Value).ToArray();
            for (var i = -1; ++i < targets.Length;) {
                targets[i].status = names.Contains(targets[i].Name) ? ImageStatus.PrepareToHide : ImageStatus.OnScreen;
            }
            Array.Sort(targets, (x, y) => x.layer - y.layer);
            return targets;
        }
        
        private void UpdateImageInfo(IList<ImageDisplayInformation> images) {
            for (var i = -1; ++i < images.Count;) {
                if (_images.ContainsKey(images[i].Name)) {
                    _images[images[i].Name] = images[i];
                } else {
                    _images.Add(images[i].Name, images[i]);
                }
            }
        }
        
        private async Task RemoveHiddenSeparateImages(string[] names) {
            if (!names.Any()) return;
            var content = new ImageMessageIntegration.HideImageContent {Names = names};
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.HideImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.HideImage, content));
            _images.RemoveAll(names);
        }
        
        private static async Task<Texture2D> BindImages(ImageDisplayInformation[] images) {
            if (images.Length == 0) return null;
            images = (await MessageService.ProcessAsync<ImageDisplayInformation[]>(
                Message<ImageDisplayInformation[]>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.UpdateInformation, images))).Content;
            var canvasSize = (await MessageService.ProcessAsync<Vector2Int>(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetCanvasSize))).Content;
            if (canvasSize.x == 0 && canvasSize.y == 0) throw new NotSupportedException("Unable to create show command: image canvas size must not be 0");
            var shader = SystemInfo.supportsComputeShaders
                ? (await MessageService.ProcessAsync(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetBindShader)) as Message<ComputeShader>)?.Content
                : null;
            var canvas = new Texture2DCombiner(canvasSize.x, canvasSize.y, shader);
            for (var i = -1; ++i < images.Length;) {
                await images[i].Content.Texture.ReadTexture();
                if (images[i].Content.Texture.texture == null) continue;
                var pivot = new Vector2(images[i].Transform?.Get(TransformValue.PropertyName.PivotX) ?? 0.0F, images[i].Transform?.Get(TransformValue.PropertyName.PivotY) ?? 0.0F);
                if (images[i].status == ImageStatus.OnScreen) {
                    if (i == 0) continue;
                    canvas.DrawTexture(images[i].Content.Texture.texture, images[i].displayMatrix, images[i].Content.Color.value, pivot, Texture2DCombiner.MixMode.RemoveMask);
                } else {
                    canvas.DrawTexture(images[i].Content.Texture.texture, images[i].displayMatrix, images[i].Content.Color.value, pivot);
                }
            }
            return canvas.Combine();
        }
        
        private static void InitializeImage(ImageDisplayInformation[] images, int layer) {
            for (var i = -1; ++i < images.Length;) {
                images[i].status = ImageStatus.PrepareToShow;
                images[i].layer = layer;
            }
        }

        private static (Texture2D Overlay, Texture2D Target, RectInt Area) CutDisplayArea(RectInt displayArea, Texture2D overlay, Texture2D target, ImageBindMode mode) {
            if (mode != ImageBindMode.Minimal) return (overlay, target, displayArea);
            RectInt actualArea;
            if (overlay == null) {
                actualArea = target.GetVisibleContentArea();
                return actualArea.Equals(displayArea) ? (overlay, target, actualArea) : (overlay, target.Cut(actualArea), actualArea);
            }
            actualArea = overlay.GetVisibleContentArea().MergeWith(target.GetVisibleContentArea());
            return actualArea.Equals(displayArea) ? (overlay, target, actualArea) : (overlay.Cut(actualArea), target.Cut(actualArea), actualArea);
        }

        private static TransformValue CreateOverlayTransform(Vector2Int position) {
            var transform = new TransformValue();
            transform.Set(TransformValue.PropertyName.PositionX, position.x);
            transform.Set(TransformValue.PropertyName.PositionY, position.y);
            transform.Set(TransformValue.PropertyName.PositionZ, 0);
            transform.Set(TransformValue.PropertyName.AnchorMinX, 0);
            transform.Set(TransformValue.PropertyName.AnchorMinY, 0);
            transform.Set(TransformValue.PropertyName.AnchorMaxX, 0);
            transform.Set(TransformValue.PropertyName.AnchorMaxY, 0);
            transform.Set(TransformValue.PropertyName.PivotX, 0);
            transform.Set(TransformValue.PropertyName.PivotY, 0);
            return transform;
        }

        private static async Task PlaceOverlayCanvas(string name, Texture2D canvas, TransformValue transform, int layer) {
            if (canvas == null) return;
            var content = new ImageMessageIntegration.ShowImageContent {
                Images = new[] {new ImageDisplayInformation(name, new ImageValue {Texture = new Texture2DValue {texture = canvas}}, transform) {layer = layer, status = ImageStatus.PrepareToShow}}
            };
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
        }

        private static async Task PlayOverlayEffect(string name, Texture2D target, SingleGraphicEffect effect, TransformValue transform, int layer) {
            var content = new ImageMessageIntegration.ShowImageContent {
                Effect = effect,
                Images = new[] {new ImageDisplayInformation(name, new ImageValue {Texture = new Texture2DValue {texture = target}}, transform) {layer = layer}}
            };
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
        }

        private static async Task PlaceNewImages(ImageDisplayInformation[] images) {
            var content = new ImageMessageIntegration.ShowImageContent {Images = images};
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
            for (var i = -1; ++i < images.Length;) {
                images[i].status = ImageStatus.OnScreen;
            }
        }

        private static async Task RemoveOverlayImage(string overlayName) {
            if (string.IsNullOrEmpty(overlayName)) return;
            var content = new ImageMessageIntegration.HideImageContent {Names = new[] {overlayName}};
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.HideImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.HideImage, content));
        }
    }
}