using System;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 表示一个编译错误
    /// </summary>
    public class CompileException : Exception {
        /// <summary>
        /// 创建一个编译错误
        /// </summary>
        /// <param name="identifier">目标文件</param>
        /// <param name="position">错误位置</param>
        /// <param name="message">错误信息</param>
        public CompileException(CodeIdentifier identifier, SourcePosition position, string message)
            :base($"{message} (at {identifier.Name}[{identifier.Hash}]:{position.Line + 1}:{position.Column + 1})") {}
    }
}
