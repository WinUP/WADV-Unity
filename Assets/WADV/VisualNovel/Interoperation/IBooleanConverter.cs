using WADV.Translation;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个布尔互操作转换器
    /// </summary>
    /// <remarks>并不是说必须实现该接口才能处理脚本中的布尔转换操作，对于没有实现该接口的值运行时会自动退化到比较实例是否为null</remarks>
    public interface IBooleanConverter {
        /// <summary>
        /// 在特定语言下获取布尔值
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage);
    }
}