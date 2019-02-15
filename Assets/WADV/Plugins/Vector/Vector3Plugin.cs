using System.Threading.Tasks;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Vector {
    [StaticRegistrationInfo("Vector3")]
    public class Vector3Plugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            float x = 0.0F, y = 0.0F, z = 0.0F;
            foreach (var (key, value) in context.StringParameters) {
                var name = key.ConvertToString(context.Language);
                switch (name) {
                    case "X":
                        x = FloatValue.TryParse(value);
                        break;
                    case "Y":
                        y = FloatValue.TryParse(value);
                        break;
                    case "Z":
                        z = FloatValue.TryParse(value);
                        break;
                        
                }
            }
            return Task.FromResult<SerializableValue>(new Vector3Value(x, y, z));
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}