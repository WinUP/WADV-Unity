using System.Collections.Generic;

namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class SequenceExpression : Expression {
        public List<Expression> Content { get; }

        public SequenceExpression(CodePosition position) : base(position) {
            Content = new List<Expression>();
        }
    }
}