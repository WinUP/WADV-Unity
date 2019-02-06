using JetBrains.Annotations;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个真值比较互操作器
    /// </summary>
    public interface IEqualOperator {
        /// <summary>
        /// 在特定语言下比较与目标值是否相等
        /// </summary>
        /// <param name="target">要比较的目标</param>
        /// <param name="language">目标语言</param>
        /// <returns>0为相等，负数为此值小于目标，其他情况为此值大于目标</returns>
        bool EqualsWith([NotNull] SerializableValue target, string language = TranslationManager.DefaultLanguage);
    }
}