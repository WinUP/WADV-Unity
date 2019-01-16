namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个布尔互操作转换器
    /// </summary>
    public interface IBooleanConverter {
        /// <summary>
        /// 获取布尔值
        /// </summary>
        /// <returns></returns>
        bool ConvertToBoolean();
        
        /// <summary>
        /// 在特定语言下获取布尔值
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        bool ConvertToBoolean(string language);
    }
}