using System.Collections.Generic;

namespace Core.VisualNovel.Runtime {
    public class Closure {
        public List<Variable> LinkedVariables { get; } = new List<Variable>();
        public List<Variable> Variables { get; } = new List<Variable>();
        public LinkedTreeNode<Closure> Node { get; set; }
    }
}