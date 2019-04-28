using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.Thread;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins {
    [StaticRegistrationInfo("Pause")]
    [UsedImplicitly]
    public class PausePlugin : IVisualNovelPlugin {
        public async Task<SerializableValue> Execute(PluginExecuteContext context) {
            float? time = null;
            foreach (var (key, value) in context.Parameters) {
                switch (key) {
                    case IFloatConverter floatConverter:
                        time = floatConverter.ConvertToFloat(context.Language);
                        break;
                    case IStringConverter stringConverter when stringConverter.ConvertToString(context.Language) == "Time":
                        time = FloatValue.TryParse(value, context.Language);
                        break;
                }
            }
            if (time != null) {
                await Dispatcher.WaitForSeconds(time.Value);
            }
            return new NullValue();
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}