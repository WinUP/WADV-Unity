namespace Core.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个静态名称参数表达式
    /// </summary>
    public class StaticNameParameterExpression : Expression {
        /// <summary>
        /// 参数名称
        /// </summary>
        public StringExpression Name { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public Expression Value { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个静态名称参数表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public StaticNameParameterExpression(SourcePosition position) : base(position) {}
    }
}