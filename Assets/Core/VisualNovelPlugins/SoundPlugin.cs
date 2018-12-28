using System;
using System.Collections.Generic;
using Core.VisualNovel.Attributes;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;
using Core.VisualNovel.Runtime.MemoryValues;

namespace Core.VisualNovelPlugins {
    [PluginTranslation("en", "Sound")]
    public class SoundPlugin : IVisualNovelPlugin {
        public string Name { get; } = "声音";

        public IEnumerable<IMemoryValue> ExecuteAsync(ExecutionContext context, IDictionary<string, IMemoryValue> parameters) {
            throw new NotImplementedException();
        }

        public IVisualNovelPlugin PickChild(ExecutionContext context, string childName) {
            throw new NotImplementedException();
        }
    }
}