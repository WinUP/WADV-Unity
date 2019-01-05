using JetBrains.Annotations;

namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个真值比较互操作器
    /// </summary>
    public interface IEqualOperator {
        /// <summary>
        /// 比较与目标值是否相等
        /// </summary>
        /// <param name="target">要比较的目标</param>
        /// <returns></returns>
        bool EqualsWith([NotNull] SerializableValue target);
    }
}