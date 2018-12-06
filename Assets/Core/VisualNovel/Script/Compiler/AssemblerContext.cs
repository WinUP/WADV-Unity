using System.Collections.Generic;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 汇编生成器上下文
    /// </summary>
    public class AssemblerContext {
        /// <summary>
        /// 汇编文件
        /// </summary>
        public AssembleFile File { get; set; } = new AssembleFile();
        /// <summary>
        /// 作用域层次
        /// </summary>
        public int Scope { get; set; }
        /// <summary>
        /// 函数列表
        /// </summary>
        public List<FunctionDescription> Functions { get; } = new List<FunctionDescription>();
        /// <summary>
        /// 获取下一个用于跳转标签的唯一ID
        /// </summary>
        public int NextLabelId {
            get {
                ++_nextLabelId;
                return _nextLabelId;
            }
        }

        private int _nextLabelId = -1;
    }
    
}