namespace WADV.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个常量表达式
    /// </summary>
    public class ConstantExpression : Expression {
        /// <summary>
        /// 常量名称
        /// </summary>
        public Expression Name { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个常量表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public ConstantExpression(SourcePosition position) : base(position) {}
    }
}