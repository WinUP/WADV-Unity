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
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Show")]
    [UsedImplicitly]
    public partial class ShowPlugin : IVisualNovelPlugin {
        private TransformValue _defaultTransform = new TransformValue();
        private int _defaultLayer = 0;
        private Dictionary<ImageProperties, ImageDisplayInformation> _images = new Dictionary<ImageProperties, ImageDisplayInformation>();

        public readonly int ShaderCanvasName = Shader.PropertyToID("Canvas");
        public readonly int ShaderCanvasRectName = Shader.PropertyToID("CanvasRect");
        public readonly int ShaderSourceName = Shader.PropertyToID("Source");
        public readonly int ShaderTransformName = Shader.PropertyToID("Transform");
        
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
            if (mode != ImageBindMode.None && images.Count > 1) {
                var preBindImages = FindPreBindImages(images.Select(e => e.Name));
                
                
                
                
                
                var getAreaMessage = await MessageService.ProcessAsync(Message<ImageMessageIntegration.GetAreaContent>.Create(
                                                                           ImageMessageIntegration.Mask,
                                                                           ImageMessageIntegration.UpdateInformation,
                                                                           new ImageMessageIntegration.GetAreaContent {Images = images}));
                if (!(getAreaMessage is Message<ImageMessageIntegration.GetAreaContent> area)) throw new NotSupportedException("Unable to create show command: only Vector2 result is acceptable for GetArea message");
                var size = area.Content.CanvasSize;
                var canvas = new Texture2D(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y));
                canvas.SetPixels(Enumerable.Repeat(Color.clear, canvas.width * canvas.height).ToArray());
                await Dispatcher.WaitAll(area.Content.Images.Select(e => e.Image.ReadTexture()));
                foreach (var (image, index) in area.Content.Images.WithIndex()) {
                    var position = area.Content.ImagePosition[index];
                    if (image.Image.texture == null) throw new NullReferenceException($"Unable to create show command: failed to load image {image.Image.source}");
                    Graphics.CopyTexture(image.Image.texture, 0, 0, 0, 0, image.Image.texture.width, image.Image.texture.height,
                                         canvas, 0, 0, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
                }
                canvas.Apply(false);
                
            }
            throw new NotSupportedException();
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
                var existed = images.FindIndex(e => e.Image.EqualsWith(currentImage, context.Language));
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

        private static async Task<Rect> GetImageContainerSize(IEnumerable<ImageDisplayInformation> images, ImageBindMode mode) {
            switch (mode) {
                case ImageBindMode.Canvas:
                    return new Rect(Vector2.zero, (await MessageService.ProcessAsync(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetCanvasSize))
                                                      as Message<Vector2>)?.Content ?? throw new ArgumentException());
                case ImageBindMode.Minimal:
                    var xMin = float.MaxValue;
                    var yMin = float.MaxValue;
                    var xMax = float.MinValue;
                    var yMax = float.MinValue;
                    foreach (var image in images) {
                        var area = image.DisplayArea;
                        xMin = area.xMin < xMin ? area.xMin : xMin;
                        yMin = area.yMin < yMin ? area.yMin : yMin;
                        xMax = area.xMax > xMax ? area.xMax : xMax;
                        yMax = area.yMax > yMax ? area.yMax : yMax;
                    }
                    return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
                default:
                    throw new NotSupportedException();
            }
        }
        
        private async Task<Texture2D> BindImages(Dictionary<ImageProperties, ImageDisplayInformation> images, ImageBindMode mode) {
            images = (await MessageService.ProcessAsync(Message<Dictionary<ImageProperties, ImageDisplayInformation>>.Create(
                                                            ImageMessageIntegration.Mask, ImageMessageIntegration.UpdateInformation, images))
                as Message<Dictionary<ImageProperties, ImageDisplayInformation>>)?.Content;
            if (images == null) throw new ArgumentException();
            var drawingArea = await GetImageContainerSize(images.Values, mode);
            if (SystemInfo.supportsComputeShaders) {
                var shader = (await MessageService.ProcessAsync(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetBindShader))
                    as Message<ComputeShader>)?.Content;
                if (shader == null) {
                    throw new NotSupportedException();
                }
                var bindKernel = shader.FindKernel("SetTexture");
                var clearKernel = shader.FindKernel("SetTransparent");
                var canvas = new RenderTexture(Mathf.RoundToInt(drawingArea.width) + 1, Mathf.RoundToInt(drawingArea.height) + 1, 24);
                shader.SetTexture(bindKernel, ShaderCanvasName, canvas);
                shader.SetTexture(clearKernel, ShaderCanvasName, canvas);
                shader.SetVector(ShaderCanvasRectName, new Vector4(drawingArea.x, drawingArea.y, drawingArea.width, drawingArea.height));
            } else {
                throw new NotSupportedException();
            }
        }
    }
}