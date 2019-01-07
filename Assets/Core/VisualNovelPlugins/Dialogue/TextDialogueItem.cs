using System.Xml;
using JetBrains.Annotations;

namespace Core.VisualNovelPlugins.Dialogue {
    /// <summary>
    /// 用于展示文本的对话框内容
    /// <para>所有样式均是对基础样式的补充或覆盖，不同TextDialogueItem之间的样式不重叠应用</para>
    /// </summary>
    public class TextDialogueItem {
        /// <summary>
        /// 字体
        /// </summary>
        [CanBeNull]
        public string FontName { get; set; }
        
        /// <summary>
        /// 字号
        /// </summary>
        [CanBeNull]
        public int? FontSize { get; set; }
        
        /// <summary>
        /// 是否为字号增减量而非字号绝对值
        /// </summary>
        [CanBeNull]
        public bool? RelativeSize { get; set; }
        
        /// <summary>
        /// 是否为粗体
        /// </summary>
        [CanBeNull]
        public bool? Bold { get; set; }
        
        /// <summary>
        /// 是否为斜体
        /// </summary>
        [CanBeNull]
        public bool? Italic { get; set; }
        
        /// <summary>
        /// 颜色
        /// </summary>
        [CanBeNull]
        public string Color { get; set; }
        
        /// <summary>
        /// 是否有删除线
        /// </summary>
        [CanBeNull]
        public bool? Strikethrough { get; set; }
        
        /// <summary>
        /// 是否有下划线
        /// </summary>
        [CanBeNull]
        public bool? Underline { get; set; }
        
        /// <summary>
        /// 文本内容
        /// </summary>
        [NotNull]
        public string Text { get; set; }

        /// <summary>
        /// 生成一个用于展示文本的对话框内容
        /// </summary>
        /// <param name="text">文本内容</param>
        public TextDialogueItem([NotNull] string text) {
            Text = text;
        }
    }
}