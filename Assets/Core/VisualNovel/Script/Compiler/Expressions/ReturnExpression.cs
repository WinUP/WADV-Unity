namespace Core.VisualNovel.Script.Compiler.Expressions {
    public class ReturnExpression : Expression{
        public Expression Value { get; set; }
        
        public ReturnExpression(CodePosition position) : base(position) {}
    }
}