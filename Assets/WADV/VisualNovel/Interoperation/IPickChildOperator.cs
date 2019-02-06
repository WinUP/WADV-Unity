using JetBrains.Annotations;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个取子元素互操作器
    /// </summary>
    public interface IPickChildOperator {
        /// <summary>
        /// 在特定语言下取出指定子元素
        /// </summary>
        /// <param name="target">目标值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        [CanBeNull]
        SerializableValue PickChild([NotNull] SerializableValue target, string language = TranslationManager.DefaultLanguage);
    }
}