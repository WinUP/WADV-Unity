using System.Collections.Generic;

namespace Core.VisualNovel.Runtime.Variables {
    public class OffsetVariable : IVariable {
        
        public Stack<CallStack> TargetStack { get; set; }
        
        public string ScriptId { get; set; }
        
        public long Offset { get; set; }
    }
}