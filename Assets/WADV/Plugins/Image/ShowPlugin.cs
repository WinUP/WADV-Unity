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

        public static readonly int ShaderCanvasName = Shader.PropertyToID("Canvas");
        public static readonly int ShaderCanvasSizeName = Shader.PropertyToID("CanvasRect");
        public static readonly int ShaderSourceName = Shader.PropertyToID("Source");
        public static readonly int ShaderTransformName = Shader.PropertyToID("Transform");
        public static readonly int ShaderColorName = Shader.PropertyToID("Color");
        
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
                var canvas = await BindImages(preBindImages, mode);
                
                
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

        private static async Task<RenderTexture> GenerateCanvasUsingShader(ComputeShader shader, Dictionary<ImageProperties, ImageDisplayInformation> images, Vector2Int canvasSize) {
            var bindKernel = shader.FindKernel("SetTexture");
            var clearKernel = shader.FindKernel("SetTransparent");
            var canvas = new RenderTexture(canvasSize.x + 2, canvasSize.y + 2, 24);
            shader.SetTexture(bindKernel, ShaderCanvasName, canvas);
            shader.SetTexture(clearKernel, ShaderCanvasName, canvas);
            shader.SetVector(ShaderCanvasSizeName, new Vector2(canvas.width, canvas.height));
            foreach (var (image, info) in images) {
                shader.SetMatrix(ShaderTransformName, info.Transform);
                await image.Image.ReadTexture();
                if (info.Status == ImageStatus.OnScreen) {
                    if (image.Image.texture != null) {
                        shader.Dispatch(clearKernel, image.Image.texture.width / 16 + 1, image.Image.texture.height / 16 + 1, 1);
                    }
                } else {
                    shader.SetTexture(bindKernel, ShaderSourceName, image.Image.texture);
                    shader.SetVector(ShaderColorName, image.Image.Color.ToVector4());
                    if (image.Image.texture != null) {
                        shader.Dispatch(bindKernel, image.Image.texture.width / 16 + 1, image.Image.texture.height / 16 + 1, 1);
                    }
                }
            }
            return canvas;
        }
        
        private static async Task<Texture2D> GenerateCanvas(Dictionary<ImageProperties, ImageDisplayInformation> images, Vector2Int canvasSize) {
            var canvas = new Texture2D(canvasSize.x, canvasSize.y, TextureFormat.RGBA32, false);
            var transparent = new Color(0, 0, 0, 0);
            foreach (var (image, info) in images) {
                await image.Image.ReadTexture();
                if (image.Image.texture == null) continue;
                var width = image.Image.texture.width;
                var height = image.Image.texture.height;
                for (var i = -1; ++i < width;) {
                    for (var j = -1; ++j < height;) {
                        var position = info.Transform * new Vector4(i, j, 0, 0);
                        if (position.x >= 0 && position.x <= canvasSize.x && position.y >= 0 && position.y <= canvasSize.y) {
                            canvas.SetPixel(i, j, info.Status == ImageStatus.OnScreen ? transparent : image.Image.texture.GetPixel(i, j));
                        }
                    }
                }
            }
            canvas.Apply(false);
            return canvas;
        }
        
        private static async Task<Texture> BindImages(Dictionary<ImageProperties, ImageDisplayInformation> images, ImageBindMode mode) {
            images = await UpdateImages(images);
            var canvasSize = await GetImageContainerSize();
            Texture canvas;
            if (SystemInfo.supportsComputeShaders) {
                var shader = (await MessageService.ProcessAsync(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetBindShader))
                    as Message<ComputeShader>)?.Content;
                if (shader == null) {
                    canvas = await GenerateCanvas(images, canvasSize);
                } else {
                    canvas = await GenerateCanvasUsingShader(shader, images, canvasSize);
                }
            } else {
                canvas = await GenerateCanvas(images, canvasSize);
            }
            if (mode == ImageBindMode.Canvas) {
                return canvas;
            }
            var original = canvas is RenderTexture renderTexture ? renderTexture.CopyAsTexture2D() : (Texture2D) canvas;
            
        }
    }
}