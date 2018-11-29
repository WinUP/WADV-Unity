namespace Core.VisualNovel.Script.Compiler.Tokens {
    public class IntegerToken : BasicToken {
        public int Content { get; set; }

        public IntegerToken(TokenType type, CodePosition position, int content) : base(type, position) {
            Content = content;
        }
    }
}