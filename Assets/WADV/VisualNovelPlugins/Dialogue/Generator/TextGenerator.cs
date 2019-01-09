using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WADV.VisualNovelPlugins.Dialogue.Generator {
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary>
    /// 表示一个对话文本生成器
    /// </summary>
    public abstract class TextGenerator : MonoBehaviour, IEnumerator<StringBuilder> {
        /// <summary>
        /// 获取或设置完整文本内容
        /// </summary>
        public abstract string Text { get; set; }
        
        public abstract bool MoveNext();

        public abstract void Reset();

        public abstract StringBuilder Current { get; }

        object IEnumerator.Current => Current;

        public abstract void Dispose();
    }
}