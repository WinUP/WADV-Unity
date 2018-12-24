using System.Collections.Generic;

namespace Core.VisualNovel.Runtime.Variables {
    public class FunctionVariable : IVariable {
        public Stack<CallStack> TargetStack { get; set; }
    }
}