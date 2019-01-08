using System.Collections.Generic;
using System.Linq;
using WADV.VisualNovel.Compiler.Tokens;

namespace WADV.VisualNovel.Compiler {
    /// <summary>
    /// 表示一个标记序列
    /// </summary>
    public class SourceTokens {
        public List<BasicToken> Content { get; }
        
        /// <summary>
        /// 当前据开头的偏移值
        /// </summary>
        public int Offset {
            get => _offset;
            set {
                if (Length == 0) {
                    return;
                }
                if (value == Offset) return;
                value = value > Length - 1 ? Length - 1 : value;
                value = value < 0 ? 0 : value;
                _offset = value;
                RecalculateTokens();
            }
        }
        /// <summary>
        /// 总Token数
        /// </summary>
        public int Length { get; }
        /// <summary>
        /// 当前操作的Token
        /// </summary>
        public BasicToken Current { get; private set; }
        /// <summary>
        /// 确定游标是否还未抵达脚本内容末尾
        /// </summary>
        public bool HasNext => Offset < Length - 1;

        private int _offset = -1;
        
        /// <summary>
        /// 创建一个标记序列
        /// </summary>
        /// <param name="tokens">标记列表</param>
        public SourceTokens(IEnumerable<BasicToken> tokens) {
            Content = tokens.ToList();
            Content.Add(new BasicToken(TokenType.LineBreak, new SourcePosition()));
            Length = Content.Count;
            MoveToNext();
        }
        
        /// <summary>
        /// 移动到下一个Token处
        /// </summary>
        /// <returns></returns>
        public void MoveToNext() {
            ++Offset;
        }
        
        /// <summary>
        /// 重置偏移值
        /// </summary>
        public void Reset() {
            Offset = -1;
            RecalculateTokens();
        }

        private void RecalculateTokens() {
            Current = Offset < 0 || Offset > Length - 1 ? null : Content[Offset];
        }
    }
}