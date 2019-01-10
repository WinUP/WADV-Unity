using System.Collections.Generic;
using WADV.VisualNovelPlugins.Dialogue.Items;

namespace WADV.VisualNovelPlugins.Dialogue {
    /// <summary>
    /// 对话框内容消息
    /// </summary>
    public class DialogueDescription {
        /// <summary>
        /// 对话角色
        /// </summary>
        public CharacterValue Character { get; set; }
        /// <summary>
        /// 对话内容
        /// </summary>
        public List<IDialogueItem> Content { get; set; }
        /// <summary>
        /// 是否为无等待对话
        /// </summary>
        public bool NoWait { get; set; }
        /// <summary>
        /// 是否为附加对话
        /// </summary>
        public bool NoClear { get; set; }
    }
}