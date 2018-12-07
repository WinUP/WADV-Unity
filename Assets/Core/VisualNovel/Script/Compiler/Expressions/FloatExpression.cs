namespace Core.VisualNovel.Script.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个32位浮点数表达式
    /// </summary>
    public class FloatExpression : Expression {
        /// <summary>
        /// 表达式数值
        /// </summary>
        public float Value { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个32位浮点数表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public FloatExpression(SourcePosition position) : base(position) {}
    }
}