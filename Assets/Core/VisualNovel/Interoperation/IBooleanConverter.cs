namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个布尔互操作转换器
    /// </summary>
    public interface IBooleanConverter {
        /// <summary>
        /// 获取布尔值
        /// </summary>
        /// <returns></returns>
        bool ConvertToBoolean();
    }
}