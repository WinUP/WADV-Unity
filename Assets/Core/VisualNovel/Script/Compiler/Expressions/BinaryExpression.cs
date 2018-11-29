using VisualNovelScript.Compiler.Expressions;

namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class BinaryExpression : Expression {
        public OperatorType Operator { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public BinaryExpression(CodePosition position) : base(position) {}
    }
}