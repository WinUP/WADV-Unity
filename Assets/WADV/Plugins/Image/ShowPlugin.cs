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
        private TransformValue _default = new TransformValue();
        private Dictionary<string, ShowingImage> _showingImages = new Dictionary<string, ShowingImage>();
        
        public async Task<SerializableValue> Execute(PluginExecuteContext context) {
            var (mode, layer, effect, images) = AnalyseParameters(context);
            var content = new ImageMessageIntegration.ShowImageContent();
            if (effect == null) {
                content.Effect = null;
            } else {
                if (!(effect.Effect is SingleGraphicEffect singleGraphicEffect)) throw new NotSupportedException($"Unable to create show command: effect {effect} is not SingleGraphicEffect");
                content.Effect = singleGraphicEffect;
            }
            if (mode != BindMode.None) {
                var getAreaMessage = await MessageService.ProcessAsync(Message<ImageMessageIntegration.GetAreaContent>.Create(
                                                                           ImageMessageIntegration.Mask,
                                                                           ImageMessageIntegration.GetArea,
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

        private (BindMode Mode, int Layer, EffectValue Effect, List<ImageInformation> Images) AnalyseParameters(PluginExecuteContext context) {
            EffectValue effect = null;
            var bind = BindMode.None;
            var layer = 0;
            var images = new List<ImageInformation>();
            ImageValue currentImage = null;
            string currentName = null;
            var currentTransform = new TransformValue();
            void AddImage() {
                var existed = images.FindIndex(e => e.Image.EqualsWith(currentImage, context.Language));
                if (existed > -1) {
                    images.RemoveAt(existed);
                }
                if (string.IsNullOrEmpty(currentName)) throw new MissingMemberException($"Unable to create show command: missing image name for {currentImage.ConvertToString(context.Language)}");
                images.Add(new ImageInformation {Name = currentName, Image = currentImage, Transform = (TransformValue) _default.AddWith(currentTransform)});
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
                                layer = FindLayer(value);
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
                                    case "Container":
                                        bind = BindMode.Canvas;
                                        break;
                                    case "Screen":
                                        bind = BindMode.Screen;
                                        break;
                                    case "Minimal":
                                    case "Min":
                                        bind = BindMode.Minimal;
                                        break;
                                    default:
                                        bind = BindMode.None;
                                        break;
                                }
                                break;
                            case "DefaultTransform":
                                _default = value is TransformValue transformValue ? transformValue : throw new ArgumentException($"Unable to set show command default transform: target {value} is not TransformValue");
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
            return (bind, layer, effect, images);
        }

        private static int FindLayer(SerializableValue value) {
            throw new NotImplementedException();
        }
    }
}