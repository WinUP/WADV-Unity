namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 表示一个脚本ID
    /// </summary>
    public class CodeIdentifier {
        /// <summary>
        /// 脚本名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 脚本哈希
        /// </summary>
        public uint Hash { get; set; }
    }
}