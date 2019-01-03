namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个32位浮点数互操作转换器
    /// </summary>
    public interface IFloatConverter {
        /// <summary>
        /// 获取32位浮点数值
        /// </summary>
        /// <returns></returns>
        float ConvertToFloat();
    }
}