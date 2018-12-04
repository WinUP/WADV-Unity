using System.Collections.Generic;

namespace Core.VisualNovel.Script.Compiler.Expressions {
    public class ScopeExpression : Expression {
        public List<Expression> Content { get; set; } = new List<Expression>();
        
        public ScopeExpression(CodePosition position) : base(position) {}
    }
}