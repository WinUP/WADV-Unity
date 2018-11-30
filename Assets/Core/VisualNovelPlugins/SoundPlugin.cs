using System.Collections.Generic;
using Core.VisualNovel;
using Core.VisualNovel.Attributes;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.StackItems;

namespace Core.VisualNovelPlugins {
    [PluginTranslation("en", "Sound", new[] {"Channel", "Resource", "FadeIn", "FadeOut", "Time", "Loop"})]
    public class SoundPlugin : IVisualNovelPlugin {
        public string Name { get; } = "声音";
        public string[] Parameters { get; } = {"声道", "资源", "淡入", "淡出", "时长", "循环"};
        public PluginIdentifier Identifier { get; } = new PluginIdentifier(0, 0, 0, 0);

        public IStackItem Execute(ExecutionContext context, IReadOnlyDictionary<int, IStackItem> parameters) {
            throw new System.NotImplementedException();
        }

        public IVisualNovelPlugin PickChild(ExecutionContext context, string childName) {
            throw new System.NotImplementedException();
        }
    }
}