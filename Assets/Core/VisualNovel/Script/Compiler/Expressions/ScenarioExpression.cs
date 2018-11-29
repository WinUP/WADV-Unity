using System.Collections.Generic;

namespace Core.VisualNovel.Script.Compiler.Expressions {
    public class ScenarioExpression : Expression {
        public string Name { get; set; }
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();
        public Expression Body { get; set; }

        public ScenarioExpression(CodePosition position) : base(position) {}
    }
}