using System.Collections.Generic;
using Core.Extensions;

namespace Core.VisualNovel.Runtime.MemoryValues {
    public class OffsetMemoryValue : IMemoryValue {
        public long Offset { get; set; }
        
        public string ScriptId { get; set; }
        
        public Stack<CallStack> RunningStack { get; set; }

        public IMemoryValue Duplicate() {
            return new OffsetMemoryValue {Offset = Offset, ScriptId = ScriptId, RunningStack = RunningStack.Duplicate()};
        }
    }
}