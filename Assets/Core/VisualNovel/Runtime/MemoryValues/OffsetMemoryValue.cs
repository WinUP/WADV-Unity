using System.Collections.Generic;

namespace Core.VisualNovel.Runtime.MemoryValues {
    public class OffsetMemoryValue : IMemoryValue {
        public long Offset { get; set; }
        public ScriptRuntime.RuntimeFile Script { get; set; }
        
        public Stack<CallStack> RunningStack { get; set; }
    }
}