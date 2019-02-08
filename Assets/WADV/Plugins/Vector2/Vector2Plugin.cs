using System.Threading.Tasks;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins.Vector2 {
    [StaticRegistrationInfo("Vector2")]
    public class Vector2Plugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            throw new System.NotImplementedException();
        }

        public void OnRegister() => true;

        public void OnUnregister(bool isReplace) => true;
    }
}