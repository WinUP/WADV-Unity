namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个加法互操作器
    /// </summary>
    public interface IAddOperator {
        /// <summary>
        /// 与目标值相加
        /// </summary>
        /// <param name="target">目标值</param>
        /// <returns></returns>
        ISerializableValue AddWith(ISerializableValue target);
    }
}