using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Plugins.Effect;
using WADV.Plugins.Image.Utilities;
using WADV.Plugins.Unity;
using WADV.Plugins.UnityUI;
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
            PluginManager.ListenerRoot.CreateChild(this);
        }

        public async Task<SerializableValue> Execute(PluginExecuteContext context) {
            var (mode, layer, effect, images) = AnalyseParameters(context);
            if (!images.Any()) return new NullValue();
            if (_placeholder != null) {
                await _placeholder;
            }
            CreatePlaceholder();
            InitializeImage(images, layer);
            if (effect == null) {
                await PlaceNewImages(images);
                UpdateImageList(images);
                CompletePlaceholder();
                return new NullValue();
            }
            var names = images.Select(e => e.Name).ToArray();
            if (mode == ImageBindMode.None) {
                var existedImages = await UpdateImageInfo(_images.Where(e => names.Contains(e.Key)).Select(e => e.Value).ToArray());
                images = await UpdateImageInfo(images);
                await Dispatcher.WaitAll(images.Select(e => e.Content.Texture.ReadTexture()));
                var targets = existedImages.Length == 0
                    ? images.Select(e => (Previous: (ImageDisplayInformation?) null, Target: e)).ToArray()
                    : (from image in images
                       join e in existedImages.AsNullable() on image.Name equals e?.Name into tempImages
                       from r in tempImages.DefaultIfEmpty()
                       select (Previous: r, Target: image)).ToArray();
                var finalImages = new List<ImageDisplayInformation>();
                for (var i = -1; ++i < targets.Length;) {
                    var (previous, target) = targets[i];
                    if (previous.HasValue && previous.Value.Content.Texture.texture != null && target.Content.Texture.texture != null) {
                        var texture = previous.Value.Content.Texture.texture;
                        var previousRect = previous.Value.displayMatrix.MultiplyRect(new Rect(0, 0, texture.width, texture.height));
                        texture = target.Content.Texture.texture;
                        var targetRect = target.displayMatrix.MultiplyRect(new Rect(0, 0, texture.width, texture.height));
                        var canvasSize = previousRect.MergeWith(targetRect).MaximizeToRectInt();
                        var shader = await GetCombinerShader();
                        var combiner = new Texture2DCombiner(canvasSize.width, canvasSize.height, shader);
                        var drawingMatrix = previous.Value.displayMatrix;
                        drawingMatrix.SetTranslation(previousRect.position - canvasSize.position);
                        combiner.DrawTexture(previous.Value.Content.Texture.texture, drawingMatrix);
                        var canvasTransform = new TransformValue()
                            .Set(TransformValue.PropertyName.PositionX, canvasSize.xMin)
                            .Set(TransformValue.PropertyName.PositionY, canvasSize.yMin);
                        await PlaceOverlayCanvas(previous.Value.Name, combiner.Combine(), canvasTransform, previous.Value.layer);
                        combiner = new Texture2DCombiner(canvasSize.width, canvasSize.height, shader);
                        drawingMatrix = target.displayMatrix;
                        drawingMatrix.SetTranslation(targetRect.position - canvasSize.position);
                        combiner.DrawTexture(target.Content.Texture.texture, drawingMatrix);
                        finalImages.Add(new ImageDisplayInformation(target.Name, new ImageValue {
                            Uv = target.Content.Uv,
                            Color = target.Content.Color,
                            Texture = new Texture2DValue {texture = combiner.Combine()}
                        }, canvasTransform) {
                            layer = target.layer,
                            status = ImageStatus.PrepareToShow
                        });
                    } else {
                        finalImages.Add(target);
                    }
                }
                await PlaceNewImages(finalImages.ToArray(), effect.Effect as SingleGraphicEffect);
                await PlaceNewImages(images);
                UpdateImageList(images);
                CompletePlaceholder();
                return new NullValue();
            }
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
            UpdateImageList(images);
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
        
        private void UpdateImageList(ImageDisplayInformation[] images) {
            for (var i = -1; ++i < images.Length;) {
                ref var image = ref images[i];
                if (_images.ContainsKey(image.Name)) {
                    _images[image.Name] = image;
                } else {
                    _images.Add(image.Name, image);
                }
            }
        }

        private async Task RemoveHiddenSeparateImages(string[] names) {
            if (!names.Any()) return;
            var content = new ImageMessageIntegration.HideImageContent {Names = names};
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.HideImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.HideImage, content));
            _images.RemoveAll(names);
        }
        
        private static async Task<ImageDisplayInformation[]> UpdateImageInfo(ImageDisplayInformation[] images) {
            return (await MessageService.ProcessAsync<ImageDisplayInformation[]>(
                Message<ImageDisplayInformation[]>.Create(
                    ImageMessageIntegration.Mask, ImageMessageIntegration.UpdateInformation, images))).Content;
        }

        private static async Task<ComputeShader> GetCombinerShader() {
            return SystemInfo.supportsComputeShaders
                ? (await MessageService.ProcessAsync(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetBindShader)) as Message<ComputeShader>)?.Content
                : null;
        }
        
        private static async Task<Texture2D> BindImages(ImageDisplayInformation[] images) {
            if (images.Length == 0) return null;
            images = await UpdateImageInfo(images);
            var canvasSize = (await MessageService.ProcessAsync<Vector2Int>(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetCanvasSize))).Content;
            if (canvasSize.x == 0 && canvasSize.y == 0) throw new NotSupportedException("Unable to create show command: image canvas size must not be 0");
            var canvas = new Texture2DCombiner(canvasSize.x, canvasSize.y, await GetCombinerShader());
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
                ref var image = ref images[i];
                image.status = ImageStatus.PrepareToShow;
                image.layer = layer;
            }
        }

        private static (Texture2D Overlay, Texture2D Target, RectInt Area) CutDisplayArea(in RectInt displayArea, Texture2D overlay, Texture2D target, ImageBindMode mode) {
            if (mode != ImageBindMode.Minimal) return (overlay, target, displayArea);
            RectInt actualArea;
            if (overlay == null) {
                actualArea = target.GetVisibleContentArea();
                return actualArea.Equals(displayArea) ? (overlay, target, actualArea) : (overlay, target.Cut(actualArea), actualArea);
            }
            actualArea = overlay.GetVisibleContentArea().MergeWith(target.GetVisibleContentArea());
            return actualArea.Equals(displayArea) ? (overlay, target, actualArea) : (overlay.Cut(actualArea), target.Cut(actualArea), actualArea);
        }

        private static TransformValue CreateOverlayTransform(in Vector2Int position) {
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

        private static async Task PlaceNewImages(ImageDisplayInformation[] images, SingleGraphicEffect effect = null) {
            var content = new ImageMessageIntegration.ShowImageContent {Images = images, Effect = effect};
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