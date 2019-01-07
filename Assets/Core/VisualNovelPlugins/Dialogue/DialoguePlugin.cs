using System.Collections.Generic;
using System.Threading.Tasks;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;
using JetBrains.Annotations;

namespace Core.VisualNovelPlugins.Dialogue {
    /// <inheritdoc />
    /// <summary>
    /// 对话解析插件
    /// </summary>
    [UsedImplicitly]
    public class DialoguePlugin : VisualNovelPlugin {
        // CJK+ASCII Range: 0020-007E,3000-30FF,31D0-31FF,4E00-9FEF,FF65-FF9F
        public DialoguePlugin() : base("Dialogue") { }
        
        public override Task<SerializableValue> Execute(ScriptRuntime context, IDictionary<SerializableValue, SerializableValue> parameters) {
            throw new System.NotImplementedException();
        }
    }
}