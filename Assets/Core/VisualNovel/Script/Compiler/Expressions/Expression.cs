namespace Core.VisualNovel.Script.Compiler.Expressions {
    public abstract class Expression {
        public CodePosition Position { get; }

        protected Expression(CodePosition position) {
            Position = position;
        }
    }
}