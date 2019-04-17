using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Plugins.Unity;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Image")]
    [UsedImplicitly]
    public class ImagePlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var result = new ImageValue();
            bool flipX = false, flipY = false;
            foreach (var (key, value) in context.StringParameters) {
                var name = key.ConvertToString(context.Language);
                switch (name) {
                    case "Texture":
                        result.Texture = value is Texture2DValue texture2DValue ? texture2DValue : throw new ArgumentException($"Unable to create image: texture {value} is not Texture2DValue");
                        break;
                    case "Color":
                        result.Color = value is ColorValue colorValue ? colorValue : throw new ArgumentException($"Unable to create image: color {value} is not ColorValue");
                        break;
                    case "Source":
                        result.Texture.source = StringValue.TryParse(value, context.Language);
                        result.Texture.texture = null;
                        break;
                    case "FlipX":
                        if (value == null || value is NullValue || BooleanValue.TryParse(value, context.Language)) {
                            flipX = true;
                        }
                        break;
                    case "FlipY":
                        if (value == null || value is NullValue || BooleanValue.TryParse(value, context.Language)) {
                            flipY = true;
                        }
                        break;
                    case "FlipXy":
                    case "FlipXY":
                    case "FlipYx":
                    case "FlipYX":
                        if (value == null || value is NullValue || BooleanValue.TryParse(value, context.Language)) {
                            flipX = flipY = true;
                        }
                        break;
                    case "Uv":
                    case "UV":
                        if (!(value is RectValue rectValue)) throw new NotSupportedException($"Unable to create image: uv {value} is not RectValue");
                        result.Uv = rectValue;
                        break;
                }
            }
            if (flipX || flipY) {
                result.Uv = new RectValue(result.Uv.value.x + (flipX ? 1 : 0),
                                          result.Uv.value.y + (flipY ? 1 : 0),
                                          result.Uv.value.width * (flipX ? -1 : 1),
                                          result.Uv.value.height * (flipY ? -1 : 1));
            }
            return Task.FromResult<SerializableValue>(result);
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}