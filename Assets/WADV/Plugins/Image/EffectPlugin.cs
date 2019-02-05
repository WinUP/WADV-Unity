using System.Threading.Tasks;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins.Image {
    public class EffectPlugin : IVisualNovelPlugin {
        /// <inheritdoc />
        public bool OnRegister() => true;

        /// <inheritdoc />
        public bool OnUnregister(bool isReplace) => true;
        
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            throw new System.NotImplementedException();
        }
    }
}