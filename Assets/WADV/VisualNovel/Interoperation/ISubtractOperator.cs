using JetBrains.Annotations;
using WADV.Translation;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个减法互操作器
    /// </summary>
    public interface ISubtractOperator {
        /// <summary>
        /// 在特定语言下与目标值相减
        /// </summary>
        /// <param name="target">目标值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        [CanBeNull]
        SerializableValue SubtractWith([NotNull] SerializableValue target, string language = TranslationManager.DefaultLanguage);
    }
}