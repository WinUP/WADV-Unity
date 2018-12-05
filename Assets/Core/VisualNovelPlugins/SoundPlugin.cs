using System;
using System.Collections.Generic;
using Core.VisualNovel;
using Core.VisualNovel.Attributes;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.StackItems;

namespace Core.VisualNovelPlugins {
    [PluginTranslation("en", "Sound")]
    public class SoundPlugin : IVisualNovelPlugin {
        public string Name { get; } = "声音";

        public IEnumerable<IStackItem> ExecuteAsync(ExecutionContext context, IReadOnlyDictionary<string, IStackItem> parameters) {
            throw new NotImplementedException();
        }

        public IVisualNovelPlugin PickChild(ExecutionContext context, string childName) {
            throw new NotImplementedException();
        }
    }
}