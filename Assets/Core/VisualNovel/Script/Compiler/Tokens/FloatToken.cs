namespace Core.VisualNovel.Script.Compiler.Tokens {
    public class FloatToken : BasicToken {
        public float Content { get; set; }

        public FloatToken(TokenType type, CodePosition position, float content) : base(type, position) {
            Content = content;
        }
    }
}