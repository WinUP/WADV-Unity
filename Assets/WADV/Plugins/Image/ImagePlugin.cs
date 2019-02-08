using System.Threading.Tasks;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Image")]
    public class ImagePlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            throw new System.NotImplementedException();
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}