using JetBrains.Annotations;

namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个可序列化值
    /// </summary>
    public interface ISerializableValue {
        /// <summary>
        /// 序列化该值
        /// </summary>
        /// <returns></returns>
        [NotNull]
        byte[] Serialize();

        /// <summary>
        /// 反序列化该值
        /// </summary>
        /// <param name="source">原始数据</param>
        void Deserialize(byte[] source);
        
        /// <summary>
        /// 获得该值的一个副本
        /// </summary>
        /// <returns></returns>
        [NotNull]
        ISerializableValue Duplicate();
    }
}