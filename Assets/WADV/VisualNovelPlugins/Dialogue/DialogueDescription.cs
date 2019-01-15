using System.Collections.Generic;
using JetBrains.Annotations;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovelPlugins.Dialogue.Items;

namespace WADV.VisualNovelPlugins.Dialogue {
    /// <inheritdoc />
    /// <summary>
    /// 对话框内容消息
    /// </summary>
    public class DialogueDescription : ContextMessageTemplate {
        /// <summary>
        /// 原始对话角色
        /// </summary>
        public SerializableValue RawCharacter { get; set; }
        
        /// <summary>
        /// 原始对话内容
        /// </summary>
        public IStringConverter RawContent { get; set; }

        public DialogueDescription(PluginExecuteContext context) : base(context) { }
    }
}