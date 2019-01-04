using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;

namespace Core.VisualNovelPlugins {
    public class SoundPlugin : VisualNovelPlugin {
        public SoundPlugin() : base("Sound") { }

        public override Task<SerializableValue> Execute(ScriptRuntime context, IDictionary<SerializableValue, SerializableValue> parameters) {
            throw new NotImplementedException();
        }
    }
}