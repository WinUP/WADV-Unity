using Core.VisualNovel.Interoperation;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="ISerializableValue" />
    /// <summary>
    /// 表示一个32位浮点数内存堆栈值
    /// </summary>
    public class BooleanMemoryValue : ISerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, IMultiplyOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public bool Value { get; set; }

        /// <inheritdoc />
        public byte[] Serialize() {
            return new[] {(byte) (Value ? 1 : 0)};
        }

        /// <inheritdoc />
        public void Deserialize(byte[] source) {
            Value = source[0] != 0;
        }

        /// <inheritdoc />
        public ISerializableValue Duplicate() {
            return new BooleanMemoryValue {Value = Value};
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return Value;
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return Value ? 1.0F : 0.0F;
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return Value ? 1 : 0;
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return Value ? "True" : "False";
        }

        /// <inheritdoc />
        public ISerializableValue AddWith(ISerializableValue target) {
            return new BooleanMemoryValue {
                Value = Value || (target is IBooleanConverter booleanTarget ? booleanTarget.ConvertToBoolean() : target != null)
            };
        }

        /// <inheritdoc />
        public ISerializableValue MultiplyWith(ISerializableValue target) {
            return new BooleanMemoryValue {
                Value = Value && (target is IBooleanConverter booleanTarget ? booleanTarget.ConvertToBoolean() : target != null)
            };
        }
    }
}