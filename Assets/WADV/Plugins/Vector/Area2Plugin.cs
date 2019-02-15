using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Vector {
    [StaticRegistrationInfo("Area2")]
    [UsedImplicitly]
    public class Area2Plugin : IVisualNovelPlugin {
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
                        (x, y) = Rect2Plugin.GetCoordinates(value, context.Language);
                        break;
                    case "Size":
                        (width, height) = Rect2Plugin.GetCoordinates(value, context.Language);
                        break;
                }
            }
            return Task.FromResult<SerializableValue>(new Area2Value(x, y, width, height));
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}