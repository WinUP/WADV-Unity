namespace Assets.Core.VisualNovel.Script {
    public class Token {
        public TokenType Type;
        public int Line;
        public int Position;
        public string Content;

        public Token(TokenType type, int line, int position, string content = "") {
            Type = type;
            Line = line;
            Position = position;
            Content = content;
        }
    }
}
