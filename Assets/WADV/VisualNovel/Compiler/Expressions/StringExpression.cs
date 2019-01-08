namespace WADV.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个字符串表达式
    /// </summary>
    public class StringExpression : Expression {
        /// <summary>
        /// 字符串值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 是否为可翻译字符串
        /// </summary>
        public bool Translatable { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个字符串表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public StringExpression(SourcePosition position) : base(position) {}
    }
}