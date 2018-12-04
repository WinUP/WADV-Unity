using System.Collections.Generic;
using Core.VisualNovel.Script.Compiler.Expressions;

namespace Core.VisualNovel.Script.Compiler {
    public class AssemblerContext {
        public AssembleFile File { get; set; } = new AssembleFile();
        public string Language { get; set; } = "default";
        public int Scope { get; set; }
        public List<ScenarioDescription> Scenarios { get; } = new List<ScenarioDescription>();
        public int NextLabelId {
            get {
                ++_nextLabelId;
                return _nextLabelId;
            }
        }

        private int _nextLabelId = -1;
    }
    
}