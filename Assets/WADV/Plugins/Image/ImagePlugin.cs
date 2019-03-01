using System;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Image")]
    public class ImagePlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var result = new ImageValue();
            foreach (var (key, value) in context.StringParameters) {
                var name = key.ConvertToString(context.Language);
                switch (name) {
                    case "Color":
                        result.color = IntegerValue.TryParse(value, context.Language);
                        break;
                    case "Duration":
                        duration = FloatValue.TryParse(value, context.Language);
                        break;
                    case "Easing": {
                        var easingName = StringValue.TryParse(value, context.Language);
                        if (!Enum.TryParse<EasingType>(easingName, true, out var easing)) {
                            throw new NotSupportedException($"Unable to create effect: ease type {easingName} is not supported");
                        }
                        easingType = easing;
                        break;
                    }
                    default:
                        parameters.Add(name, value);
                        break;
                }
            }
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}