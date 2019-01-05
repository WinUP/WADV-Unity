using JetBrains.Annotations;

namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个比较互操作器
    /// </summary>
    public interface ICompareOperator {
        /// <summary>
        /// 与目标比较大小
        /// </summary>
        /// <param name="target">要比较的目标</param>
        /// <returns>0为相等，负数为此值小于目标，其他情况为此值大于目标</returns>
        int CompareWith([CanBeNull] SerializableValue target);
    }
}