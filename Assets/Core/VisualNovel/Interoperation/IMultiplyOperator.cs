using JetBrains.Annotations;

namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个乘法互操作器
    /// </summary>
    public interface IMultiplyOperator {
        /// <summary>
        /// 与目标值相乘
        /// </summary>
        /// <param name="target">目标值</param>
        /// <returns></returns>
        [CanBeNull]
        SerializableValue MultiplyWith([NotNull] SerializableValue target);
    }
}