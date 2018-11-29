namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class ToBooleanExpression : Expression {
        public Expression Value { get; set; }

        public ToBooleanExpression(CodePosition position) : base(position) {}
    }
}