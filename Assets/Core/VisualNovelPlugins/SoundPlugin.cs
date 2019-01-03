using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;

namespace Core.VisualNovelPlugins {
    public class SoundPlugin : IVisualNovelPlugin {
        public string Name { get; } = "Sound";

        public Task<ISerializableValue> Execute(ScriptRuntime context, IDictionary<ISerializableValue, ISerializableValue> parameters) {
            throw new NotImplementedException();
        }

        public IVisualNovelPlugin PickChild(ScriptRuntime context, string childName) {
            throw new NotImplementedException();
        }

        public ISerializableValue ToValue() {
            throw new NotImplementedException();
        }
    }
}