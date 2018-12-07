using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.VisualNovel.Script.Compiler;
using Core.VisualNovel.Script.Compiler.Expressions;
using Core.VisualNovel.Script.Compiler.Tokens;
using UnityEngine;

namespace Core.VisualNovel.Script {
    public class ScriptCompiler {
        public string Source { get; }
        public CodeIdentifier Identifier { get; }
        public IReadOnlyCollection<BasicToken> Tokens { get; private set; }
        public ScopeExpression RootExpression { get; private set; }
        
        public ScriptCompiler(string path) {
            Source = Resources.Load<TextAsset>(path)?.text;
            if (Source == null) {
                throw new FileNotFoundException($"Cannot find unity resource file {path}");
            }
            Identifier = new CodeIdentifier {Name = path, Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(Source))};
        }

        public ScriptCompiler Lex() {
            Tokens = new Lexer(Source, Identifier).Lex();
            return this;
        }

        public ScriptCompiler Parse() {
            if (Tokens == null) {
                throw new ArgumentException("Must finish lex before create AST");
            }
            RootExpression = new Parser(Tokens, Identifier).Parse();
            return this;
        }
    }
}