namespace Core.VisualNovel.Script.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个快捷对话表达式
    /// </summary>
    public class DialogueExpression : Expression {
        /// <summary>
        /// 对话角色
        /// </summary>
        public string Character { get; set; }
        /// <summary>
        /// 对话内容
        /// </summary>
        public string Content { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个快捷对话表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public DialogueExpression(SourcePosition position) : base(position) {}
    }
}