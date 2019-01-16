namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个32位浮点数互操作转换器
    /// </summary>
    public interface IFloatConverter {
        /// <summary>
        /// 获取32位浮点数值
        /// </summary>
        /// <returns></returns>
        float ConvertToFloat();
        
        /// <summary>
        /// 在特定语言下获取32位浮点数值
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        float ConvertToFloat(string language);
    }
}