namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class ConditionContentExpression : Expression {
        public Expression Condition { get; set; }
        public Expression Body { get; set; }
        
        public ConditionContentExpression(CodePosition position) : base(position) {}
    }
}