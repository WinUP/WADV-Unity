using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.VisualNovel.Runtime.MemoryValues;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 表示一个运行时错误
    /// </summary>
    public class RuntimeException : Exception {
        private readonly List<ScopeMemoryValue> _callStack;
        
        /// <summary>
        /// 创建一个运行时错误
        /// </summary>
        /// <param name="callStack">调用堆栈</param>
        /// <param name="message">错误信息</param>
        public RuntimeException(IEnumerable<ScopeMemoryValue> callStack, string message) : base(message) {
            _callStack = callStack.ToList();
        }

        public override string StackTrace {
            get {
                var result = new StringBuilder();
                // 写入脚本调用堆栈
                foreach (var callStack in _callStack) {
                    var position = ScriptHeader.LoadAsset(callStack.ScriptId).Header.Positions[callStack.Entrance];
                    result.AppendLine($"   at {callStack.ScriptId}.vns:{position.Line}, {position.Column}");
                }
                return result.ToString();
            }
        }
    }
}