namespace Core.VisualNovel.Runtime.StackItems {
    public interface IStackItem {}

    public interface IStackItem<T> : IStackItem {
        T Content { get; set; }
    }
}