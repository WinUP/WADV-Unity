namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class Expression {
        public CodePosition Position { get; set; }

        public Expression(CodePosition position) {
            Position = position;
        }
    }
}