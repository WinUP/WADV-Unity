namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个32位整数互操作转换器
    /// </summary>
    public interface IIntegerConverter {
        /// <summary>
        /// 获取32位整数值
        /// </summary>
        /// <returns></returns>
        int ConvertToInteger();
    }
}