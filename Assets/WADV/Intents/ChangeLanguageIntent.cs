using WADV.VisualNovel.Runtime;

namespace WADV.Intents {
    /// <summary>
    /// 更改执行语言请求
    /// </summary>
    public struct ChangeLanguageIntent {
        /// <summary>
        /// 目标脚本执行环境
        /// </summary>
        public ScriptRuntime Runtime { get; set; }
        
        /// <summary>
        /// 新的语言
        /// </summary>
        public string NewLanguage { get; set; }
    }
}