namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个字符串互操作转换器
    /// </summary>
    public interface IStringConverter {
        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <returns></returns>
        string ConvertToString();
    }
}