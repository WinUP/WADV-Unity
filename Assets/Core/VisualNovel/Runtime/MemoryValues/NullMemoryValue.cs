namespace Core.VisualNovel.Runtime.MemoryValues {
    public class NullMemoryValue : IMemoryValue {
        public IMemoryValue Duplicate() {
            return new NullMemoryValue();
        }
    }
}