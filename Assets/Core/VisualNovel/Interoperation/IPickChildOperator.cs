namespace Core.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个取子元素互操作器
    /// </summary>
    public interface IPickChildOperator {
        SerializableValue PickChild(SerializableValue name);
    }
}