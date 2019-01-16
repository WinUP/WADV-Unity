using System;
using WADV.Extensions;
using WADV.VisualNovel.Interoperation;
using UnityEngine;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个字符串内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>布尔转换器</description></item>
    ///     <item><description>浮点转换器</description></item>
    ///     <item><description>整数转换器</description></item>
    ///     <item><description>字符串转换器</description></item>
    ///     <item><description>加法互操作器</description></item>
    ///     <item><description>减法互操作器</description></item>
    ///     <item><description>乘法互操作器</description></item>
    ///     <item><description>除法互操作器</description></item>
    ///     <item><description>真值比较互操作器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>子元素/特性支持</description></listheader>
    ///     <item><description>ToBoolean</description></item>
    ///     <item><description>ToNumber</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class StringValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                               IEqualOperator, IPickChildOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new StringValue {Value = Value};
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            var upperValue = Value.ToUpper();
            if (upperValue == "F" || upperValue == "FALSE") return false;
            if (int.TryParse(Value, out var intValue) && intValue == 0) return false;
            return !(float.TryParse(Value, out var floatValue) && floatValue.Equals(0.0F));
        }

        /// <inheritdoc />
        public bool ConvertToBoolean(string language) {
            return ConvertToBoolean();
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            if (float.TryParse(Value, out var floatValue)) return floatValue;
            return Value == "" ? 0.0F : 1.0F;
        }

        /// <inheritdoc />
        public float ConvertToFloat(string language) {
            return ConvertToFloat();
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            if (int.TryParse(Value, out var intValue)) return intValue;
            return Value == "" ? 0 : 1;
        }

        /// <inheritdoc />
        public int ConvertToInteger(string language) {
            return ConvertToInteger();
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return Value;
        }
        
        /// <inheritdoc />
        public string ConvertToString(string language) {
            return ConvertToString();
        }

        /// <inheritdoc />
        public override string ToString() {
            return ConvertToString();
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
        public bool EqualsWith(SerializableValue target, string language) {
            return EqualsWith(target);
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString() : target.ToString();
            return new StringValue {Value = $"{Value}{targetString}"};
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target, string language) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString(language) : target.ToString();
            return new StringValue {Value = $"{Value}{targetString}"};
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString() : target.ToString();
            return new StringValue {Value = Value.Replace(targetString, "")};
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target, string language) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString(language) : target.ToString();
            return new StringValue {Value = Value.Replace(targetString, "")};
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return new StringValue {Value = Value.Repeat(intTarget.ConvertToInteger())};
                case IFloatConverter floatTarget:
                    return new StringValue {Value = Value.Repeat(Mathf.RoundToInt(floatTarget.ConvertToFloat()))};
                case IStringConverter _:
                    throw new NotSupportedException("Unable to multiply string constant with string constant");
                case IBooleanConverter boolTarget:
                    return new StringValue {Value = boolTarget.ConvertToBoolean() ? Value : ""};
                default:
                    throw new NotSupportedException($"Unable to multiply string constant with unsupported value {target}");
            }
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target, string language) {
            return MultiplyWith(target);
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return new StringValue {Value = DivideString(Value, intTarget.ConvertToInteger())};
                case IFloatConverter floatTarget:
                    return new StringValue {Value = DivideString(Value, Mathf.RoundToInt(floatTarget.ConvertToFloat()))};
                case IStringConverter _:
                    throw new NotSupportedException("Unable to divide string constant with string constant");
                case IBooleanConverter boolTarget:
                    return new StringValue {Value = boolTarget.ConvertToBoolean() ? Value : ""};
                default:
                    throw new NotSupportedException($"Unable to divide string constant with unsupported value {target}");
            }
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target, string language) {
            return DivideWith(target);
        }
        
        /// <inheritdoc />
        public SerializableValue PickChild(SerializableValue name) {
            if (!(name is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in string value with feature id {name}: only string feature name is accepted");
            var target = stringConverter.ConvertToString();
            switch (target) {
                case "ToBoolean":
                    return new BooleanValue {Value = ConvertToBoolean()};
                case "ToNumber":
                    return new IntegerValue {Value = ConvertToInteger()};
                default:
                    throw new NotSupportedException($"Unable to get feature in string value: unsupported feature {target}");
            }
        }
        
        /// <inheritdoc />
        public SerializableValue PickChild(SerializableValue target, string language) {
            return PickChild(target);
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