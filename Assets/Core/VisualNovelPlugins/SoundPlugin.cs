using System;
using System.Collections.Generic;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;
using Core.VisualNovel.Runtime.MemoryValues;

namespace Core.VisualNovelPlugins {
    public class SoundPlugin : IVisualNovelPlugin {
        public string Name { get; } = "Sound";

        public IEnumerable<IMemoryValue> ExecuteAsync(ExecutionContext context, IDictionary<string, IMemoryValue> parameters) {
            throw new NotImplementedException();
        }

        public IVisualNovelPlugin PickChild(ExecutionContext context, string childName) {
            throw new NotImplementedException();
        }
    }
}