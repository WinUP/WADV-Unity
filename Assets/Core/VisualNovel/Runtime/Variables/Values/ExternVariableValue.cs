namespace Core.VisualNovel.Runtime.Variables.Values {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个托管数据变量值
    /// </summary>
    public class ExternVariableValue : IVariableValue {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public object Value { get; set; }

        /// <inheritdoc />
        public IVariableValue Duplicate() {
            return new ExternVariableValue {Value = Value};
        }
    }
}