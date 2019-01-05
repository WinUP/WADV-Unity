using System;
using Core.Extensions;
using Core.VisualNovel.Interoperation;
using UnityEngine;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// 表示一个字符串内存堆栈值
    /// </summary>
    [Serializable]
    public class StringMemoryValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                                     IEqualOperator{
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new StringMemoryValue {Value = Value};
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            var upperValue = Value.ToUpper();
            if (upperValue == "F" || upperValue == "FALSE") return false;
            if (int.TryParse(Value, out var intValue) && intValue == 0) return false;
            return !(float.TryParse(Value, out var floatValue) && floatValue.Equals(0.0F));
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            if (float.TryParse(Value, out var floatValue)) return floatValue;
            return Value == "" ? 0.0F : 1.0F;
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            if (int.TryParse(Value, out var intValue)) return intValue;
            return Value == "" ? 0 : 1;
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return Value;
        }

        public override string ToString() {
            return $"StringMemoryValue {{Value = {ConvertToString()}}}";
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target) {
            switch (target) {
                case IStringConverter stringConverter:
                    return stringConverter.ConvertToString() == Value;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString() : target.ToString();
            return new StringMemoryValue {Value = $"{Value}{targetString}"};
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString() : target.ToString();
            return new StringMemoryValue {Value = Value.Replace(targetString, "")};
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return new StringMemoryValue {Value = Value.Repeat(intTarget.ConvertToInteger())};
                case IFloatConverter floatTarget:
                    return new StringMemoryValue {Value = Value.Repeat(Mathf.RoundToInt(floatTarget.ConvertToFloat()))};
                case IStringConverter _:
                    throw new NotSupportedException("Unable to multiply string constant with string constant");
                case IBooleanConverter boolTarget:
                    return new StringMemoryValue {Value = boolTarget.ConvertToBoolean() ? Value : ""};
                default:
                    throw new NotSupportedException($"Unable to multiply string constant with unsupported value {target}");
            }
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return new StringMemoryValue {Value = DivideString(Value, intTarget.ConvertToInteger())};
                case IFloatConverter floatTarget:
                    return new StringMemoryValue {Value = DivideString(Value, Mathf.RoundToInt(floatTarget.ConvertToFloat()))};
                case IStringConverter _:
                    throw new NotSupportedException("Unable to divide string constant with string constant");
                case IBooleanConverter boolTarget:
                    return new StringMemoryValue {Value = boolTarget.ConvertToBoolean() ? Value : ""};
                default:
                    throw new NotSupportedException($"Unable to divide string constant with unsupported value {target}");
            }
        }

        private static string DivideString(string source, int length) {
            if (length == source.Length) return source;
            if (length > source.Length) throw new NotSupportedException($"Unable to divide string to target length {length}");
            var endIndex = Mathf.RoundToInt(source.Length / (float) length);
            if (endIndex > source.Length) {
                endIndex = source.Length;
            }
            return source.Substring(0, endIndex);
        }
    }
}