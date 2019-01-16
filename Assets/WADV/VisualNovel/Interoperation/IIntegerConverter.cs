namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个32位整数互操作转换器
    /// </summary>
    public interface IIntegerConverter {
        /// <summary>
        /// 获取32位整数值
        /// </summary>
        /// <returns></returns>
        int ConvertToInteger();
        
        /// <summary>
        /// 在特定语言下获取32位整数值
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        int ConvertToInteger(string language);
    }
}