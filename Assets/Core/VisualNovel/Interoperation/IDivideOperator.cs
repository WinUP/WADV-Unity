namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个除法互操作器
    /// </summary>
    public interface IDivideOperator {
        /// <summary>
        /// 与目标值相除
        /// </summary>
        /// <param name="target">目标值</param>
        /// <returns></returns>
        SerializableValue DivideWith(SerializableValue target);
    }
}