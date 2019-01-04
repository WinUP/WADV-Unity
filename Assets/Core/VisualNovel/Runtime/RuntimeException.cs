using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 表示一个运行时错误
    /// </summary>
    public class RuntimeException : Exception {
        private readonly List<CallStack> _scope;
        
        /// <summary>
        /// 创建一个运行时错误
        /// </summary>
        /// <param name="scope">调用堆栈</param>
        /// <param name="message">错误信息</param>
        public RuntimeException(IEnumerable<CallStack> scope, string message) : base(message) {
            _scope = scope.Reverse().ToList();
        }

        public RuntimeException(IEnumerable<CallStack> scope, Exception innerException) : base("Unexpected runtime exception occured, see inner exception for more information", innerException) {
            _scope = scope.Reverse().ToList();
        }

        public override string StackTrace {
            get {
                var result = new StringBuilder();
                // 写入脚本调用堆栈
                foreach (var scope in _scope) {
                    var position = ScriptHeader.LoadAsset(scope.ScriptId).Header.Positions[scope.Offset];
                    result.AppendLine($"   at {scope.ScriptId}: Line {position.Line}, Column {position.Column}");
                }
                return result.ToString();
            }
        }
    }
}