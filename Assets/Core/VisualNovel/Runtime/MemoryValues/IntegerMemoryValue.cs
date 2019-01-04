using System;
using Core.VisualNovel.Interoperation;
using UnityEngine;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// 表示一个32位整数内存堆栈值
    /// </summary>
    [Serializable]
    public class IntegerMemoryValue : SerializableValue, IBooleanConverter, IIntegerConverter, IFloatConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public int Value { get; set; }

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new IntegerMemoryValue {Value = Value};
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return Value != 0;
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return Value;
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return Value;
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return Value.ToString();
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return new IntegerMemoryValue {Value = Value + ToInteger(target)};
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            return new IntegerMemoryValue {Value = Value - ToInteger(target)};
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new IntegerMemoryValue {Value = Value * ToInteger(target)};
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            return new IntegerMemoryValue {Value = Value / ToInteger(target)};
        }

        private static int ToInteger(SerializableValue target) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return intTarget.ConvertToInteger();
                case IFloatConverter floatTarget:
                    return Mathf.RoundToInt(floatTarget.ConvertToFloat());
                case IStringConverter stringTarget:
                    var stringValue = stringTarget.ConvertToString();
                    if (int.TryParse(stringValue, out var intValue)) return intValue;
                    if (float.TryParse(stringValue, out var floatValue)) return Mathf.RoundToInt(floatValue);
                    throw new NotSupportedException($"Unable to add integer with unsupported string format {stringValue}");
                case IBooleanConverter boolTarget:
                    return boolTarget.ConvertToBoolean() ? 1 : 0;
                default:
                    throw new NotSupportedException($"Unable to add integer with unsupported value {target}");
            }
        }
    }
}