namespace Core.VisualNovel.Script.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个逻辑取反表达式
    /// </summary>
    public class LogicNotExpression : Expression {
        /// <summary>
        /// 要取反的内容
        /// </summary>
        public Expression Content { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个逻辑取反表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public LogicNotExpression(SourcePosition position) : base(position) {}
    }
}