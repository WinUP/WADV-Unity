namespace Core.VisualNovel.Script.Compiler.Expressions {
    public class IntegerExpression : Expression {
        public int Value { get; set; }

        public IntegerExpression(CodePosition position) : base(position) {}
    }
}