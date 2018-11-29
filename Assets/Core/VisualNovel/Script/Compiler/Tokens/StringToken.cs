namespace Core.VisualNovel.Script.Compiler.Tokens {
    public class StringToken : BasicToken {
        public string Content { get; set; }
        public bool Translatable { get; set; }

        public StringToken(TokenType type, CodePosition position, string content, bool translatable = false) : base(type, position) {
            Content = content;
            Translatable = translatable;
        }
    }
}