namespace Core.VisualNovel.Script.Compiler.Expressions {
    public class StringExpression : Expression {
        public string Value { get; set; }
        public bool Translatable { get; set; }
        
        public StringExpression(CodePosition position) : base(position) {}
    }
}