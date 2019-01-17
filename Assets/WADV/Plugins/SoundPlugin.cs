using System;
using System.Threading.Tasks;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins {
    public class SoundPlugin : VisualNovelPlugin {
        public SoundPlugin() : base("Sound") { }

        public override Task<SerializableValue> Execute(PluginExecuteContext context) {
            throw new NotImplementedException();
        }
    }
}