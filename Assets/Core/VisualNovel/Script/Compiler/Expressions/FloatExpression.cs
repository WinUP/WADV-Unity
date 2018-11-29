namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class FloatExpression : Expression {
        public float Value { get; set; }

        public FloatExpression(CodePosition position) : base(position) {}
    }
}