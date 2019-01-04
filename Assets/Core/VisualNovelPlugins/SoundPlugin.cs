using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;

namespace Core.VisualNovelPlugins {
    public class SoundPlugin : IVisualNovelPlugin {
        public string Name { get; } = "Sound";

        public Task<SerializableValue> Execute(ScriptRuntime context, IDictionary<SerializableValue, SerializableValue> parameters) {
            throw new NotImplementedException();
        }

        public IVisualNovelPlugin PickChild(ScriptRuntime context, string childName) {
            throw new NotImplementedException();
        }

        public SerializableValue ToValue() {
            throw new NotImplementedException();
        }
    }
}