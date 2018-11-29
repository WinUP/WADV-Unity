using System.Collections.Generic;
using VisualNovelScript.Compiler.Expressions;

namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class ConditionExpression : Expression {
        public List<ConditionContentExpression> Contents { get; } = new List<ConditionContentExpression>();

        public ConditionExpression(CodePosition position) : base(position) {}
    }
}