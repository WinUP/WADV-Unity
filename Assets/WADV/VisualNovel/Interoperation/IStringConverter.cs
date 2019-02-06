using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个字符串互操作转换器
    /// </summary>
    public interface IStringConverter {
        /// <summary>
        /// 在特定语言下获取字符串值
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        string ConvertToString(string language = TranslationManager.DefaultLanguage);
    }
}