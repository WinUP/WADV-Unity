namespace Core.VisualNovel.Script.Compiler.Tokens {
    public class BooleanToken : BasicToken {
        public bool Content { get; set; }

        public BooleanToken(TokenType type, CodePosition position, bool content) : base(type, position) {
            Content = content;
        }
    }
}