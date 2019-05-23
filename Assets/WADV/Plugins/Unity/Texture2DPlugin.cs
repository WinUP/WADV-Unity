using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Unity {
    [StaticRegistrationInfo("Texture2D")]
    [UsedImplicitly]
    public class Texture2DPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var result = new Texture2DValue();
            foreach (var (key, value) in context.StringParameters) {
                var name = key.ConvertToString(context.Language);
                switch (name) {
                    case "Source":
                        result.source = StringValue.TryParse(value, context.Language);
                        break;
                }
            }
            return Task.FromResult<SerializableValue>(result);
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}