using System.Collections.Generic;
using Core.Extensions;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个偏移量内存堆栈值
    /// </summary>
    public class OffsetMemoryValue : IMemoryValue {
        /// <summary>
        /// 获取或设置偏移量
        /// </summary>
        public long Offset { get; set; }
        
        /// <summary>
        /// 获取或设置目标脚本ID
        /// </summary>
        public string ScriptId { get; set; }
        
        /// <summary>
        /// 获取或设置应用此偏移量时使用的执行堆栈
        /// </summary>
        public Stack<CallStack> RunningStack { get; set; }

        /// <inheritdoc />
        public IMemoryValue Duplicate() {
            return new OffsetMemoryValue {Offset = Offset, ScriptId = ScriptId, RunningStack = RunningStack.Duplicate()};
        }
    }
}