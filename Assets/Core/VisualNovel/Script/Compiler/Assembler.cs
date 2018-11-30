using Core.VisualNovel.Script.Compiler.Expressions;

namespace Core.VisualNovel.Script.Compiler {
    public class Assembler {
        public Expression RootExpression { get; }
        public string Identifier { get; }
        
        private AssembleFile File { get; set; }
        private string Language { get; set; }

        public Assembler(Expression root, string identifier) {
            RootExpression = root;
            Identifier = identifier;
        }

        public void Assemble() {
            File = new AssembleFile();
            Language = "default";
        }

        private void AssembleLanguage(LanguageExpression expression) {
            File.Language(Language = expression.Language);
        }

        private void AssembleDialogue(DialogueExpression expression) {
            File.LoadDialogue(expression.Character, expression.Content);
        }
    }
}