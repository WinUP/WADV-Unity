namespace Core.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个脚本语言切换表达式
    /// </summary>
    public class LanguageExpression : Expression {
        /// <summary>
        /// 目标语言
        /// </summary>
        public string Language { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个脚本语言切换表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public LanguageExpression(SourcePosition position) : base(position) {}
    }
}