namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 变量搜索模式
    /// </summary>
    public enum VariableSearchMode {
        /// <summary>
        /// 搜索所有变量
        /// </summary>
        All,
        /// <summary>
        /// 仅搜索常量
        /// </summary>
        OnlyConstant,
        /// <summary>
        /// 仅搜索非常量
        /// </summary>
        OnlyNonConstant
    }
}