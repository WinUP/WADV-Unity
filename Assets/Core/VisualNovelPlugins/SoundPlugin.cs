using System;
using System.Collections.Generic;
using Core.VisualNovel;
using Core.VisualNovel.Attributes;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime.Variable;
using Core.VisualNovel.Runtime.Variables;

namespace Core.VisualNovelPlugins {
    [PluginTranslation("en", "Sound")]
    public class SoundPlugin : IVisualNovelPlugin {
        public string Name { get; } = "声音";

        public IEnumerable<IVariable> ExecuteAsync(ExecutionContext context, IDictionary<string, IVariable> parameters) {
            throw new NotImplementedException();
        }

        public IVisualNovelPlugin PickChild(ExecutionContext context, string childName) {
            throw new NotImplementedException();
        }
    }
}