namespace Core.VisualNovel.Runtime.Variables.Values {
    /// <summary>
    /// 表示一个变量值
    /// </summary>
    public interface IVariableValue {
        /// <summary>
        /// 获取此变量值的一个拷贝
        /// </summary>
        /// <returns></returns>
        IVariableValue Duplicate();
    }
}