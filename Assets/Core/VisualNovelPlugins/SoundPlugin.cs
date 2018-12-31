using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;
using Core.VisualNovel.Runtime.MemoryValues;

namespace Core.VisualNovelPlugins {
    public class SoundPlugin : IVisualNovelPlugin {
        public string Name { get; } = "Sound";

        public Task<IMemoryValue> ExecuteAsync(ScriptRuntime context, IDictionary<IMemoryValue, IMemoryValue> parameters) {
            throw new NotImplementedException();
        }

        public IVisualNovelPlugin PickChild(ScriptRuntime context, string childName) {
            throw new NotImplementedException();
        }
    }
}