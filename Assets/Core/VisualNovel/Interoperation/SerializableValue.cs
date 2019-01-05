using System;
using JetBrains.Annotations;

namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个可序列化内存值
    /// </summary>
    [Serializable]
    public abstract class SerializableValue {
        protected SerializableValue() {
            if (!GetType().IsSerializable) throw new NotSupportedException($"Missing Serializable attribute: {GetType().FullName} is not serializable value");
        }
        
        /// <summary>
        /// 获得该值的一个副本
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public abstract SerializableValue Duplicate();
    }
}