using System;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.Plugins.Unity;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Image")]
    public class ImagePlugin : IVisualNovelPlugin {
        private static readonly Rect FlipX = new Rect(1, 0, -1, 0);
        private static readonly Rect FlipY = new Rect(0, 1, 1, -1);
        private static readonly Rect FlipXy = new Rect(1, 1, -1, -1);
        
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var result = new ImageValue();
            foreach (var (key, value) in context.StringParameters) {
                var name = key.ConvertToString(context.Language);
                switch (name) {
                    case "Color":
                        result.Color = value is ColorValue colorValue ? colorValue : throw new ArgumentException($"Unable to create image: {value} is not ColorValue");
                        break;
                    case "Source":
                        result.source = StringValue.TryParse(value, context.Language);
                        break;
                    case "FlipX": {
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