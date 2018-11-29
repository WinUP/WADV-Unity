namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class ScopeExpression : Expression {
        public Expression Content { get; set; }
        
        public ScopeExpression(CodePosition position) : base(position) {}
    }
}