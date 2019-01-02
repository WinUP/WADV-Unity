namespace Core.VisualNovel.Runtime.Variables.Values {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个布尔变量值
    /// </summary>
    public class BooleanVariableValue : IVariableValue {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public bool Value { get; set; }

        /// <inheritdoc />
        public IVariableValue Duplicate() {
            return new BooleanVariableValue {Value = Value};
        }
    }
}