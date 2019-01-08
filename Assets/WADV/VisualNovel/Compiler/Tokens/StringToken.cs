namespace WADV.VisualNovel.Compiler.Tokens {
    /// <inheritdoc />
    /// <summary>
    /// 创建一个字符串标记
    /// </summary>
    public class StringToken : BasicToken {
        /// <summary>
        /// 字符串内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否为可翻译字符串
        /// </summary>
        public bool Translatable { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个字符串标记
        /// </summary>
        /// <param name="type">标记类型</param>
        /// <param name="position">该标记在源代码中的对应位置</param>
        /// <param name="content">字符串内容</param>
        /// <param name="translatable">是否为可翻译字符串</param>
        public StringToken(TokenType type, SourcePosition position, string content, bool translatable) : base(type, position) {
            Content = content;
            Translatable = translatable;
        }
    }
}