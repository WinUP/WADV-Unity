namespace Core.VisualNovel.Plugin {
    /// <summary>
    /// 表示一个WADV插件ID
    /// <para>同一个项目中插件ID是不允许重复的，插件作者请务必慎重填写</para>
    /// </summary>
    public struct PluginIdentifier {
        /// <summary>
        /// ID第一部分
        /// </summary>
        public byte Part1 { get; }
        /// <summary>
        /// ID第二部分
        /// </summary>
        public byte Part2 { get; }
        /// <summary>
        /// ID第三部分
        /// </summary>
        public byte Part3 { get; }
        /// <summary>
        /// ID第四部分
        /// </summary>
        public byte Part4 { get; }

        /// <summary>
        /// 表示一个WADV插件ID
        /// </summary>
        /// <param name="part1">ID第一部分</param>
        /// <param name="part2">ID第二部分</param>
        /// <param name="part3">ID第三部分</param>
        /// <param name="part4">ID第四部分</param>
        public PluginIdentifier(byte part1, byte part2, byte part3, byte part4) {
            Part1 = part1;
            Part2 = part2;
            Part3 = part3;
            Part4 = part4;
        }

        /// <summary>
        /// 比较两个插件ID是否相同
        /// </summary>
        /// <param name="target">目标插件ID</param>
        /// <returns></returns>
        public bool IsSameId(PluginIdentifier target) {
            return target.Part1 == Part1 && target.Part2 == Part2 && target.Part3 == Part3 && target.Part4 == Part4;
        }
    }
}