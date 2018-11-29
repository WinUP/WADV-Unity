namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class LoopExpression : Expression {
        public Expression Condition { get; set; }
        public Expression Body { get; set; }

        public LoopExpression(CodePosition position) : base(position) {}
    }
}