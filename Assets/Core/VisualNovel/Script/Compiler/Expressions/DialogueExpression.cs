namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class DialogueExpression : Expression {
        public string Character { get; set; }
        public string Content { get; set; }

        public DialogueExpression(CodePosition position) : base(position) {}
    }
}