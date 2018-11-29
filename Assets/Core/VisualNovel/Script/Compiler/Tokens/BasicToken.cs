namespace Core.VisualNovel.Script.Compiler.Tokens {
    public class BasicToken {
        public TokenType Type { get; }
        public CodePosition Position { get; }

        public BasicToken(TokenType type, CodePosition position) {
            Type = type;
            Position = position;
        }
    }
}
