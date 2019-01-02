namespace Core.VisualNovel.Runtime.Variables.Values {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个字符串变量值
    /// </summary>
    public class StringVariableValue : IVariableValue {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc />
        public IVariableValue Duplicate() {
            return new StringVariableValue {Value = Value};
        }
    }
}