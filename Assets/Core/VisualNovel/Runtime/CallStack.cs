using System.Collections.Generic;
using Core.VisualNovel.Runtime.Variables;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 表示一层作用域
    /// </summary>
    public class CallStack {
        /// <summary>
        /// 该作用域内的局部常量/变量
        /// </summary>
        public Dictionary<string, Variable> Variables { get; } = new Dictionary<string, Variable>();
        /// <summary>
        /// 该作用域使用的脚本文件
        /// </summary>
        public string ScriptId { get; set; }
        /// <summary>
        /// 该作用域入口在脚本代码段中的偏移值
        /// </summary>
        public long Offset { get; set; }
    }
}