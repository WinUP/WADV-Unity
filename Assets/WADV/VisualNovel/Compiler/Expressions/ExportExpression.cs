namespace WADV.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个导出表达式
    /// </summary>
    public class ExportExpression : Expression {
        /// <summary>
        /// 导出项的值
        /// </summary>
        public Expression Value { get; set; }
        /// <summary>
        /// 导出项名
        /// </summary>
        public Expression Name { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个导出表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public ExportExpression(SourcePosition position) : base(position) {}
    }
}