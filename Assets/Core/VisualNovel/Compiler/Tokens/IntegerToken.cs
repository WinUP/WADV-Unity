namespace Core.VisualNovel.Compiler.Tokens {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个32位整数标记
    /// </summary>
    public class IntegerToken : BasicToken {
        /// <summary>
        /// 整数值
        /// </summary>
        public int Content { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个32位整数标记
        /// </summary>
        /// <param name="type">标记类型</param>
        /// <param name="position">该标记在源代码中的对应位置</param>
        /// <param name="content">整数值</param>
        public IntegerToken(TokenType type, SourcePosition position, int content) : base(type, position) {
            Content = content;
        }
    }
}