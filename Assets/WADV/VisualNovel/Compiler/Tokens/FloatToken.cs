namespace WADV.VisualNovel.Compiler.Tokens {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个32位浮点数标记
    /// </summary>
    public class FloatToken : BasicToken {
        /// <summary>
        /// 浮点数值
        /// </summary>
        public float Content { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个32位浮点数标记
        /// </summary>
        /// <param name="type">标记类型</param>
        /// <param name="position">该标记在源代码中的对应位置</param>
        /// <param name="content">浮点数值</param>
        public FloatToken(TokenType type, SourcePosition position, float content) : base(type, position) {
            Content = content;
        }
    }
}