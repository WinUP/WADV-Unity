using System;

namespace Core.VisualNovel.Script.Compiler {
    public class CompileException : Exception {
        public CompileException(string file, CodePosition position, string message)
            :base($"{message} \n  at Script {file}:{position.Line + 1}:{position.Column + 1}") {}
    }
}
