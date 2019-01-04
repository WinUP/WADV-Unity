using System;
using System.Globalization;
using Core.VisualNovel.Interoperation;
using UnityEngine;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// 表示一个32位浮点数内存堆栈值
    /// </summary>
    [Serializable]
    public class FloatMemoryValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public float Value { get; set; }

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new FloatMemoryValue {Value = Value};
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return !Value.Equals(0.0F);
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return Value;
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return Mathf.RoundToInt(Value);
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString() {
            return $"FloatMemoryValue {{Value = {ConvertToString()}}}";
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return new FloatMemoryValue {Value = Value + ToFloat(target)};
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            return new FloatMemoryValue {Value = Value - ToFloat(target)};
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new FloatMemoryValue {Value = Value * ToFloat(target)};
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            return new FloatMemoryValue {Value = Value / ToFloat(target)};
        }
        
        private static float ToFloat(SerializableValue target) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return intTarget.ConvertToInteger();
                case IFloatConverter floatTarget:
                    return floatTarget.ConvertToFloat();
                case IStringConverter stringTarget:
                    var stringValue = stringTarget.ConvertToString();
                    if (int.TryParse(stringValue, out var intValue)) return intValue;
                    if (float.TryParse(stringValue, out var floatValue)) return Mathf.RoundToInt(floatValue);
                    throw new NotSupportedException($"Unable to add float with unsupported string format {stringValue}");
                case IBooleanConverter boolTarget:
                    return boolTarget.ConvertToBoolean() ? 1.0F : 0.0F;
                default:
                    throw new NotSupportedException($"Unable to add float with unsupported value {target}");
            }
        }
    }
}