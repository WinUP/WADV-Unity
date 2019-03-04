using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Plugins.Unity;
using WADV.Plugins.Vector;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
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
                    case "Color":
                        result.Color = value is ColorValue colorValue ? colorValue : throw new ArgumentException($"Unable to create image: color {value} is not ColorValue");
                        break;
                    case "Source":
                        result.source = StringValue.TryParse(value, context.Language);
                        break;
                    case "FlipX":
                        flipX = true;
                        break;
                    case "FlipY":
                        flipY = true;
                        break;
                    case "FlipXy":
                    case "FlipXY":
                        flipX = flipY = true;
                        break;
                    case "Uv":
                    case "UV":
                        if (!(value is RectValue rectValue)) throw new NotSupportedException($"Unable to create image: uv {value} is not RectValue");
                        result.Uv = rectValue;
                        break;
                }
            }
            if (flipX) {
                result.Uv = new RectValue(result.Uv.value.x + 1, result.Uv.value.y, result.Uv.value.width * -1, result.Uv.value.height);
            }
            if (flipY) {
                result.Uv = new RectValue(result.Uv.value.x, result.Uv.value.y + 1, result.Uv.value.width, result.Uv.value.height * -1);
            }
            return Task.FromResult<SerializableValue>(result);
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}