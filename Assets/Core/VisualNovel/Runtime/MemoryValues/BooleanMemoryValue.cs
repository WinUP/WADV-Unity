using System;
using Core.VisualNovel.Interoperation;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// 表示一个32位浮点数内存堆栈值
    /// </summary>
    [Serializable]
    public class BooleanMemoryValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, IMultiplyOperator,
                                      IPickChildOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public bool Value { get; set; }

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
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

        public override string ToString() {
            return $"BooleanMemoryValue {{Value = {ConvertToString()}}}";
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return new BooleanMemoryValue {
                Value = Value || (target is IBooleanConverter booleanTarget ? booleanTarget.ConvertToBoolean() : target != null)
            };
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new BooleanMemoryValue {
                Value = Value && (target is IBooleanConverter booleanTarget ? booleanTarget.ConvertToBoolean() : target != null)
            };
        }

        /// <inheritdoc />
        public SerializableValue PickChild(SerializableValue name) {
            if (!(name is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in boolean variable with feature id {name}: Only string feature name is accepted");
            var target = stringConverter.ConvertToString();
            switch (target) {
                case "Reverse":
                    return new BooleanMemoryValue {Value = !Value};
                case "ToNumber":
                    return new IntegerMemoryValue {Value = ConvertToInteger()};
                case "ToString":
                    return new StringMemoryValue {Value = ConvertToString()};
                default:
                    throw new NotSupportedException($"Unable to get feature in boolean variable with feature id {target}: Unsupported feature");
            }
        }
    }
}