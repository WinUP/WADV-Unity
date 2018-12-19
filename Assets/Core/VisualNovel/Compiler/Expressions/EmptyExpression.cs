namespace Core.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个空表达式
    /// </summary>
    public class EmptyExpression : Expression {
        /// <inheritdoc />
        /// <summary>
        /// 创建一个空表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public EmptyExpression(SourcePosition position) : base(position) {}
    }
}