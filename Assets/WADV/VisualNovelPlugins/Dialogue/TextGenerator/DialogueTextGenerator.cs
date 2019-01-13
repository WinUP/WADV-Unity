using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WADV.VisualNovelPlugins.Dialogue.TextGenerator {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个对话文本生成器
    /// </summary>
    public abstract class DialogueTextGenerator : IEnumerator<StringBuilder> {
        /// <summary>
        /// 创建对话文本生成器
        /// </summary>
        /// <param name="type">生成器类型</param>
        /// <returns></returns>
        public static DialogueTextGenerator Create(DialogueTextGeneratorType type) {
            switch (type) {
                case DialogueTextGeneratorType.None:
                    return new EmptyDialogueTextGenerator();
                case DialogueTextGeneratorType.Simple:
                    return new SimpleDialogueTextGenerator();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, $"Unable to create dialogue text generator: unknown type {type}");
            }
        }
        
        /// <summary>
        /// 获取或设置完整文本内容
        /// </summary>
        public abstract string Text { get; set; }
        
        /// <summary>
        /// 生成下一个文字
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();

        /// <summary>
        /// 重置生成器
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// 获取当前已生成的文本
        /// </summary>
        public abstract StringBuilder Current { get; }

        object IEnumerator.Current => Current;

        /// <summary>
        /// 释放生成器资源
        /// </summary>
        public abstract void Dispose();
    }
}