using Core.VisualNovel.Runtime.Variables.Values;

namespace Core.VisualNovel.Runtime.Variables {
    public class ValueVariable : IVariable {
        public IVariableValue Value { get; set; }
    }
}