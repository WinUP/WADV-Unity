namespace Core.VisualNovel.Runtime.Variables.Values {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个空变量值
    /// </summary>
    public class NullVariableValue : IVariableValue {
        /// <inheritdoc />
        public IVariableValue Duplicate() {
            return new NullVariableValue();
        }
    }
}