namespace Core.VisualNovel.Runtime.MemoryValues {
    public class StaticMemoryValue<T> : IMemoryValue {
        public T Value { get; set; }
    }
}