using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime;

namespace WADV.VisualNovelPlugins {
    public class SoundPlugin : VisualNovelPlugin {
        public SoundPlugin() : base("Sound") { }

        public override Task<SerializableValue> Execute(PluginExecuteContext context) {
            throw new NotImplementedException();
        }
    }
}