using System.Collections.Generic;
using Core.Extensions;

namespace Core.VisualNovel.Runtime.Variables.Values {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个偏移量变量值
    /// </summary>
    public class OffsetVariableValue : IVariableValue {
        /// <summary>
        /// 获取或设置应用此偏移量时使用的执行堆栈
        /// </summary>
        public Stack<CallStack> RunningStack { get; set; }
        
        /// <summary>
        /// 获取或设置目标脚本ID
        /// </summary>
        public string ScriptId { get; set; }
        
        /// <summary>
        /// 获取或设置偏移量
        /// </summary>
        public long Offset { get; set; }
        
        /// <inheritdoc />
        public IVariableValue Duplicate() {
            return new OffsetVariableValue {
                RunningStack = RunningStack.Duplicate(),
                ScriptId = ScriptId,
                Offset = Offset
            };
        }
    }
}