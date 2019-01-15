using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WADV.VisualNovel.Runtime {
    /// <summary>
    /// 表示一个运行时错误
    /// </summary>
    public class RuntimeException : Exception {
        private readonly List<CallStack.StackItem> _scope;
        
        /// <summary>
        /// 创建一个运行时错误
        /// </summary>
        /// <param name="scope">调用堆栈</param>
        /// <param name="message">错误信息</param>
        public RuntimeException(IEnumerable<CallStack.StackItem> scope, string message) : base(message) {
            _scope = scope.Reverse().ToList();
        }

        public RuntimeException(IEnumerable<CallStack.StackItem> scope, Exception innerException) : base(innerException.Message, innerException) {
            _scope = scope.Reverse().ToList();
        }

        public override string StackTrace {
            get {
                var result = new StringBuilder();
                // 写入脚本调用堆栈
                foreach (var scope in _scope) {
                    var position = ScriptHeader.LoadAsset(scope.ScriptId).Header.Positions[scope.Offset];
                    result.AppendLine($"   at {scope.ScriptId}: Line {position.Line + 1}, Column {position.Column + 1}");
                }
                return result.ToString();
            }
        }
    }
}