using System;
using System.Collections.Generic;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 表示一个编译选项
    /// </summary>
    [Serializable]
    public class ScriptCompileOption {
        /// <summary>
        /// 编译时除了默认语言外需要额外生成或更新的翻译文件
        /// </summary>
        public List<string> ExtraTranslationLanguages { get; } = new List<string>();

        public uint SourceHash { get; set; }
        
        public uint? BinaryHash { get; set; }
    }
}