using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Converter {
    [StaticRegistrationInfo("Integer")]
    [UsedImplicitly]
    public class IntegerPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            foreach (var (key, value) in context.Parameters) {
                return Task.FromResult<SerializableValue>(new IntegerValue {
                    value = IntegerValue.TryParse(
                        key is IStringConverter stringKey && stringKey.ConvertToString(context.Language) == "Value" ? value : key
                    )
                });
            }
            return Task.FromResult<SerializableValue>(new IntegerValue {value = 0});
        }

        public void OnRegister() { } 

        public void OnUnregister(bool isReplace) { }
    }
}