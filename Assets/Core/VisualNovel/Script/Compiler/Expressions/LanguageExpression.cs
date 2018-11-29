namespace Assets.Core.VisualNovel.Script.Compiler.Expressions {
    public class LanguageExpression : Expression {
        public string Language { get; set; }
        
        public LanguageExpression(CodePosition position) : base(position) {}
    }
}