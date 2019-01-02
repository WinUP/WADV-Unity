namespace Core.VisualNovel.Runtime.Variables.Values {
    /// <summary>
    /// 表示一个32位整数变量值
    /// </summary>
    public class IntegerVariableValue : IVariableValue {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public int Value { get; set; }

        /// <inheritdoc />
        public IVariableValue Duplicate() {
            return new IntegerVariableValue {Value = Value};
        }
    }
}