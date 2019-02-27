using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Vector {
    [StaticRegistrationInfo("Rect2")]
    [UsedImplicitly]
    public class Rect2Plugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            float x = 0.0F, y = 0.0F, width = 0.0F, height = 0.0F;
            foreach (var (key, value) in context.StringParameters) {
                switch (key.ConvertToString(context.Language)) {
                    case "X":
                        x = FloatValue.TryParse(value, context.Language);
                        break;
                    case "Y":
                        y = FloatValue.TryParse(value, context.Language);
                        break;
                    case "Width":
                        width = FloatValue.TryParse(value, context.Language);
                        break;
                    case "Height":
                        height = FloatValue.TryParse(value, context.Language);
                        break;
                    case "Position":
                        (x, y) = GetCoordinates(value, context.Language);
                        break;
                    case "Size":
                        (width, height) = GetCoordinates(value, context.Language);
                        break;
                }
            }
            return Task.FromResult<SerializableValue>(new Rect2Value(x, y, width, height));
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }

        public static (float X, float Y) GetCoordinates(SerializableValue target, string language) {
            switch (target) {
                case Vector3Value vector3Value:
                    return (vector3Value.value.x, vector3Value.value.y);
                case Vector2Value vector2Value:
                    return (vector2Value.value.x, vector2Value.value.y);
                case Rect2Value rect2Value:
                    return (rect2Value.value.x, rect2Value.value.y);
                case IFloatConverter floatConverter:
                    var number = floatConverter.ConvertToFloat(language);
                    return (number, number);
                default:
                    throw new NotSupportedException($"Unable to get coordinates from {target}: unsupported type");
            }
        }
    }
}