namespace WADV.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个布尔转换表达式
    /// </summary>
    public class ToBooleanExpression : Expression {
        /// <summary>
        /// 要转换的值
        /// </summary>
        public Expression Value { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个布尔转换表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public ToBooleanExpression(SourcePosition position) : base(position) {}
    }
}