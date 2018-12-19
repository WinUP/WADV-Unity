namespace Core.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个脚本引用表达式
    /// </summary>
    public class ImportExpression : Expression {
        /// <summary>
        /// 引用目标相对于当前文件的路径，或引用目标以Assets/Resources为根目录时的路径（以/开头）
        /// </summary>
        public Expression Target { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个脚本引用表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public ImportExpression(SourcePosition position) : base(position) {}
    }
}