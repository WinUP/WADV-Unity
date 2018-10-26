using System;

namespace Assets.Core.VisualNovel.Script {
    public class TokenizerException : Exception {
        public TokenizerException(string file, int line, int position, string message)
            :base($"{file} {line}:{position} {message}") { }
    }
}
