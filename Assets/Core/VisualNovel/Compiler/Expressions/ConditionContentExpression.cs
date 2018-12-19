namespace Core.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个条件分支内容表达式
    /// </summary>
    public class ConditionContentExpression : Expression {
        /// <summary>
        /// 分支条件
        /// </summary>
        public Expression Condition { get; set; }
        /// <summary>
        /// 分支内容
        /// </summary>
        public Expression Body { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个条件分支内容表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public ConditionContentExpression(SourcePosition position) : base(position) {}
    }
}