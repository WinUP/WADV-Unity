namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class VariableExpression : Expression {
        public Expression Name { get; set; }
        
        public VariableExpression(CodePosition position) : base(position) {}
    }
}