using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 表示一个运行时错误
    /// </summary>
    public class RuntimeException : Exception {
        private readonly List<CallStack> _callStack;
        
        /// <summary>
        /// 创建一个运行时错误
        /// </summary>
        /// <param name="callStack">调用堆栈</param>
        /// <param name="message">错误信息</param>
        public RuntimeException(IEnumerable<CallStack> callStack, string message) : base(message) {
            _callStack = callStack.ToList();
        }

        public override string StackTrace {
            get {
                var stack = (new StackTrace().GetFrames() ?? new StackFrame[] { }).ToList();
                // 写入脚本调用堆栈
                foreach (var callStack in _callStack) {
                    var position = callStack.Script.DebugPositions[callStack.Offset];
                    stack.Insert(0, new StackFrame(callStack.Script.Id, position.Line, position.Column));
                }
                return base.StackTrace;
            }
        }
    }
}