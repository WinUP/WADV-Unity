namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class LogicNotExpression : Expression {
        public Expression Content { get; set; }
        
        public LogicNotExpression(CodePosition position) : base(position) {}
    }
}