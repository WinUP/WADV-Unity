namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <summary>
    /// 表示一个内存堆栈值
    /// </summary>
    public interface IMemoryValue {
        /// <summary>
        /// 获取此内存堆栈值的一个拷贝
        /// </summary>
        /// <returns></returns>
        IMemoryValue Duplicate();
    }
}