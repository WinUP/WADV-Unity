using System.Collections.Generic;

namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class CommandExpression : Expression {
        public Expression Target { get; set; }
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();
        
        public CommandExpression(CodePosition position) : base(position) {}
    }
}