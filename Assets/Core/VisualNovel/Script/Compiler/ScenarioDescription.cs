using Core.VisualNovel.Script.Compiler.Expressions;

namespace Core.VisualNovel.Script.Compiler {
    public class ScenarioDescription {
        public FunctionExpression Function { get; set; }
        public string Label { get; set; }
        public long Offset { get; set; }
        public int Scope { get; set; }
    }
}