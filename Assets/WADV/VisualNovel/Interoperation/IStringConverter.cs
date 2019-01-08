namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个字符串互操作转换器
    /// </summary>
    public interface IStringConverter {
        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <returns></returns>
        string ConvertToString();

        /// <summary>
        /// 获取也定语言下的字符串值
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        string ConvertToString(string language);
    }
}