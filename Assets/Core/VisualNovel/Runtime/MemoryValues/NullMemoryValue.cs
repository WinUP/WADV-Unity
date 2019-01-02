namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个空内存堆栈值
    /// </summary>
    public class NullMemoryValue : IMemoryValue {
        /// <inheritdoc />
        public IMemoryValue Duplicate() {
            return new NullMemoryValue();
        }
    }
}