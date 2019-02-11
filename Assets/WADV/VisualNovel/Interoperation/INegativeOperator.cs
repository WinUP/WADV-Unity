using JetBrains.Annotations;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个取反互操作器
    /// </summary>
    public interface INegativeOperator {
        /// <summary>
        /// 在特定语言下对此值取反
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        [CanBeNull]
        SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage);
    }
}