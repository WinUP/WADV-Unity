using System.Collections.Generic;

namespace Core.VisualNovel.Runtime.Variables {
    public class FunctionVariable : IVariable {
        public Stack<CallStack> TargetStack { get; set; }
        public ScriptRuntime.RuntimeFile Script { get; set; }
        public long Offset { get; set; }
    }
}