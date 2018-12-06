namespace Core.VisualNovel.Script.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个二元运算符表达式
    /// </summary>
    public class BinaryExpression : Expression {
        /// <summary>
        /// 表达式运算符
        /// </summary>
        public OperatorType Operator { get; set; }
        /// <summary>
        /// 运算符左侧表达式
        /// </summary>
        public Expression Left { get; set; }
        /// <summary>
        /// 运算符右侧表达式
        /// </summary>
        public Expression Right { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个二元运算符表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public BinaryExpression(CodePosition position) : base(position) {}
    }
}