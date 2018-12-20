using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 表示一个运行时错误
    /// </summary>
    public class RuntimeException : Exception {
        private readonly List<(ScriptRuntime.RuntimeFile Script, long Position)> _callStack;
        
        /// <summary>
        /// 创建一个运行时错误
        /// </summary>
        /// <param name="callStack">调用堆栈</param>
        /// <param name="message">错误信息</param>
        public RuntimeException(IEnumerable<(ScriptRuntime.RuntimeFile Script, long Position)> callStack, string message) : base(message) {
            _callStack = callStack.ToList();
        }

        public override string StackTrace {
            get {
                var stack = (new StackTrace().GetFrames() ?? new StackFrame[] { }).ToList();
                foreach (var (script, offset) in _callStack) {
                    var position = script.DebugPositions[offset];
                    stack.Insert(0, new StackFrame(script.Id, position.Line, position.Column));
                }
                return string.Join("\n", stack.Select(e => $"  at {e.GetFileName()} {e.GetFileLineNumber()}:{e.GetFileColumnNumber()}"));
            }
        }
    }
}