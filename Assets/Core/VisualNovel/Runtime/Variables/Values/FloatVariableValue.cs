namespace Core.VisualNovel.Runtime.Variables.Values {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个32位浮点数变量值
    /// </summary>
    public class FloatVariableValue : IVariableValue {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public float Value { get; set; }

        /// <inheritdoc />
        public IVariableValue Duplicate() {
            return new FloatVariableValue {Value = Value};
        }
    }
}