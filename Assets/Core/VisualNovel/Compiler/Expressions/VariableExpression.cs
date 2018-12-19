namespace Core.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个变量表达式
    /// </summary>
    public class VariableExpression : Expression {
        /// <summary>
        /// 变量名称
        /// </summary>
        public Expression Name { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个变量表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public VariableExpression(SourcePosition position) : base(position) {}
    }
}