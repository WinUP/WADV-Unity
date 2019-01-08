namespace WADV.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个返回表达式
    /// </summary>
    public class ReturnExpression : Expression{
        /// <summary>
        /// 返回内容
        /// </summary>
        public Expression Value { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个返回表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public ReturnExpression(SourcePosition position) : base(position) {}
    }
}