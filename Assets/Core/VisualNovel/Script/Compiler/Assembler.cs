using System;
using Core.VisualNovel.Script.Compiler.Expressions;

namespace Core.VisualNovel.Script.Compiler {
    public class Assembler {
        public Expression RootExpression { get; }
        public string Identifier { get; }

        public Assembler(Expression root, string identifier) {
            RootExpression = root;
            Identifier = identifier;
        }

        public (byte[] code, string translate) Assemble() {
            throw new NotImplementedException();
        }
    }
}