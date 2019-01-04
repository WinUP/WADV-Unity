using System;
using Core.VisualNovel.Interoperation;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// 表示一个32位浮点数内存堆栈值
    /// </summary>
    [Serializable]
    public class BooleanMemoryValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, IMultiplyOperator {
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
    }
}