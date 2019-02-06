using System;
using WADV.Extensions;
using WADV.VisualNovel.Interoperation;
using UnityEngine;
using WADV.VisualNovel.Translation;

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

        public override SerializableValue Duplicate() {
            return new StringValue {Value = Value};
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            var upperValue = Value.ToUpper();
            if (upperValue == "F" || upperValue == "FALSE") return false;
            if (int.TryParse(Value, out var intValue) && intValue == 0) return false;
            return !(float.TryParse(Value, out var floatValue) && floatValue.Equals(0.0F));
        }

        public float ConvertToFloat(string language = TranslationManager.DefaultLanguage) {
            if (float.TryParse(Value, out var floatValue)) return floatValue;
            return Value == "" ? 0.0F : 1.0F;
        }

        public int ConvertToInteger(string language = TranslationManager.DefaultLanguage) {
            if (int.TryParse(Value, out var intValue)) return intValue;
            return Value == "" ? 0 : 1;
        }
        
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return Value;
        }

        public override string ToString() {
            return ConvertToString();
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case IStringConverter stringConverter:
                    return stringConverter.ConvertToString(language) == Value;
                default:
                    return false;
            }
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString(language) : target.ToString();
            return new StringValue {Value = $"{Value}{targetString}"};
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString(language) : target.ToString();
            return new StringValue {Value = Value.Replace(targetString, "")};
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return new StringValue {Value = Value.Repeat(intTarget.ConvertToInteger(language))};
                case IFloatConverter floatTarget:
                    return new StringValue {Value = Value.Repeat(Mathf.RoundToInt(floatTarget.ConvertToFloat(language)))};
                case IStringConverter _:
                    throw new NotSupportedException("Unable to multiply string constant with string constant");
                case IBooleanConverter boolTarget:
                    return new StringValue {Value = boolTarget.ConvertToBoolean(language) ? Value : ""};
                default:
                    throw new NotSupportedException($"Unable to multiply string constant with unsupported value {target}");
            }
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return new StringValue {Value = DivideString(Value, intTarget.ConvertToInteger(language))};
                case IFloatConverter floatTarget:
                    return new StringValue {Value = DivideString(Value, Mathf.RoundToInt(floatTarget.ConvertToFloat(language)))};
                case IStringConverter _:
                    throw new NotSupportedException("Unable to divide string constant with string constant");
                case IBooleanConverter boolTarget:
                    return new StringValue {Value = boolTarget.ConvertToBoolean(language) ? Value : ""};
                default:
                    throw new NotSupportedException($"Unable to divide string constant with unsupported value {target}");
            }
        }
        
        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in string value with feature id {target}: only string feature name is accepted");
            var name = stringConverter.ConvertToString(language);
            switch (name) {
                case "ToBoolean":
                    return new BooleanValue {Value = ConvertToBoolean(language)};
                case "ToNumber":
                    return new IntegerValue {Value = ConvertToInteger(language)};
                default:
                    throw new NotSupportedException($"Unable to get feature in string value: unsupported feature {name}");
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