namespace Core.VisualNovel.Script.Compiler.Expressions {
    public class ParameterExpression : Expression {
        public Expression Name { get; set; }
        public Expression Value { get; set; }
        
        public ParameterExpression(CodePosition position) : base(position) {}
    }
}