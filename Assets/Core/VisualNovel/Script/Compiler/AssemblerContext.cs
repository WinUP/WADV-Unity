using System.Collections.Generic;
using Core.VisualNovel.Script.Compiler.Expressions;

namespace Core.VisualNovel.Script.Compiler {
    public class AssemblerContext {
        public AssembleFile File { get; } = new AssembleFile();
        public string Language { get; set; } = "default";
        public int Scopes { get; set; }
        public List<ScenarioExpression> Scenarios { get; } = new List<ScenarioExpression>();
        public int NextLabelId {
            get {
                ++_nextLabelId;
                return _nextLabelId;
            }
        }

        private int _nextLabelId = -1;
    }
    
}