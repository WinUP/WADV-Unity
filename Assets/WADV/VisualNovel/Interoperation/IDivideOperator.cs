using JetBrains.Annotations;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个除法互操作器
    /// </summary>
    public interface IDivideOperator {
        /// <summary>
        /// 在特定语言下与目标值相除
        /// </summary>
        /// <param name="target">目标值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        [CanBeNull]
        SerializableValue DivideWith([NotNull] SerializableValue target, string language = TranslationManager.DefaultLanguage);
    }
}