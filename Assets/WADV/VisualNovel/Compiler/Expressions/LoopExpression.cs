namespace WADV.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个循环表达式
    /// </summary>
    public class LoopExpression : Expression {
        /// <summary>
        /// 循环条件
        /// </summary>
        public Expression Condition { get; set; }
        /// <summary>
        /// 循环内容
        /// </summary>
        public Expression Body { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个循环表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public LoopExpression(SourcePosition position) : base(position) {}
    }
}