using System.Collections.Generic;

namespace Core.VisualNovel.Runtime.MemoryValues {
    public class OffsetMemoryValue : IMemoryValue {
        public long Offset { get; set; }
        
        public string ScriptId { get; set; }
        
        public Stack<CallStack> RunningStack { get; set; }
    }
}