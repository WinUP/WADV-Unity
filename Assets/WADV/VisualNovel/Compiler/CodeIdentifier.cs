namespace WADV.VisualNovel.Compiler {
    /// <summary>
    /// 表示一个脚本ID
    /// </summary>
    public class CodeIdentifier {
        /// <summary>
        /// 脚本ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 脚本哈希
        /// </summary>
        public uint Hash { get; set; }
    }
}