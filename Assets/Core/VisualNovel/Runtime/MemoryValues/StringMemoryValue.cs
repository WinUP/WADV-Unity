using System;
using Core.Extensions;
using Core.VisualNovel.Interoperation;
using UnityEngine;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="ISerializableValue" />
    /// <summary>
    /// 表示一个字符串内存堆栈值
    /// </summary>
    public class StringMemoryValue : ISerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public string Value { get; set; }

        public byte[] Serialize() {
            throw new NotImplementedException();
        }

        public void Deserialize(byte[] source) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ISerializableValue Duplicate() {
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

        /// <inheritdoc />
        public ISerializableValue AddWith(ISerializableValue target) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString() : target.ToString();
            return new StringMemoryValue {Value = $"{Value}{targetString}"};
        }

        /// <inheritdoc />
        public ISerializableValue SubtractWith(ISerializableValue target) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString() : target.ToString();
            return new StringMemoryValue {Value = Value.Replace(targetString, "")};
        }

        /// <inheritdoc />
        public ISerializableValue MultiplyWith(ISerializableValue target) {
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
        public ISerializableValue DivideWith(ISerializableValue target) {
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