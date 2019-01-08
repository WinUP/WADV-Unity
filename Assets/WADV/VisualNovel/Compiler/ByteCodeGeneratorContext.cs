namespace WADV.VisualNovel.Compiler {
    /// <summary>
    /// 字节码生成器上下文
    /// </summary>
    public class ByteCodeGeneratorContext {
        /// <summary>
        /// 汇编文件
        /// </summary>
        public ByteCodeWriter File { get; set; } = new ByteCodeWriter();
        /// <summary>
        /// 作用域层次
        /// </summary>
        public int Scope { get; set; }
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