using System;
using System.Globalization;
using WADV.Extensions;
using WADV.VisualNovel.Interoperation;
using UnityEngine;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="IBooleanConverter" />
    /// <inheritdoc cref="IFloatConverter" />
    /// <inheritdoc cref="IIntegerConverter" />
    /// <inheritdoc cref="IStringConverter" />
    /// <inheritdoc cref="IAddOperator" />
    /// <inheritdoc cref="ISubtractOperator" />
    /// <inheritdoc cref="IMultiplyOperator" />
    /// <inheritdoc cref="IDivideOperator" />
    /// <inheritdoc cref="INegativeOperator" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <summary>
    /// <para>表示一个字符串内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>引用复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>1 字符串</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>类型转换支持</description></listheader>
    ///     <item><description>布尔转换器</description></item>
    ///     <item><description>浮点转换器</description></item>
    ///     <item><description>整数转换器</description></item>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>加法互操作器</description></item>
    ///     <item><description>减法互操作器</description></item>
    ///     <item><description>乘法互操作器</description></item>
    ///     <item><description>除法互操作器</description></item>
    ///     <item><description>取反互操作器</description></item>
    ///     <item><description>相等比较互操作器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class StringValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                               IEqualOperator, INegativeOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public string value;
        
        /// <summary>
        /// 尝试将可序列化值解析为32为浮点数值
        /// </summary>
        /// <param name="value">目标内存值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static string TryParse(SerializableValue value, string language = TranslationManager.DefaultLanguage) {
            switch (value) {
                case IFloatConverter floatTarget:
                    return floatTarget.ConvertToFloat(language).ToString(CultureInfo.InvariantCulture);
                case IIntegerConverter intTarget:
                    return intTarget.ConvertToInteger(language).ToString(CultureInfo.InvariantCulture);
                case IStringConverter stringTarget:
                    return stringTarget.ConvertToString(language);
                case IBooleanConverter boolTarget:
                    return boolTarget.ConvertToBoolean(language) ? "True" : "False";
                default:
                    throw new NotSupportedException($"Unable to convert {value} to string: unsupported format");
            }
        }

        public override SerializableValue Duplicate() {
            return new StringValue {value = value};
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            var upperValue = value.ToUpper();
            if (upperValue == "F" || upperValue == "FALSE") return false;
            if (int.TryParse(upperValue, out var intValue) && intValue == 0) return false;
            return !(float.TryParse(upperValue, out var floatValue) && floatValue.Equals(0.0F));
        }

        public float ConvertToFloat(string language = TranslationManager.DefaultLanguage) {
            if (float.TryParse(value, out var floatValue)) return floatValue;
            return value == "" ? 0.0F : 1.0F;
        }

        public int ConvertToInteger(string language = TranslationManager.DefaultLanguage) {
            if (int.TryParse(value, out var intValue)) return intValue;
            return value == "" ? 0 : 1;
        }
        
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return value;
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new BooleanValue {value = !ConvertToBoolean(language)};
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case IStringConverter stringConverter:
                    return stringConverter.ConvertToString(language) == value;
                default:
                    return false;
            }
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString(language) : target.ToString();
            return new StringValue {value = $"{value}{targetString}"};
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var targetString = target is IStringConverter stringTarget ? stringTarget.ConvertToString(language) : target.ToString();
            return new StringValue {value = value.Replace(targetString, "")};
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return new StringValue {value = value.Repeat(intTarget.ConvertToInteger(language))};
                case IFloatConverter floatTarget:
                    return new StringValue {value = value.Repeat(Mathf.RoundToInt(floatTarget.ConvertToFloat(language)))};
                case IStringConverter stringConverter:
                    throw new NotSupportedException($"Unable to multiply string {value} with {stringConverter.ConvertToString(language)}: string cannot multiply with string");
                case IBooleanConverter boolTarget:
                    return new StringValue {value = boolTarget.ConvertToBoolean(language) ? value : ""};
                default:
                    throw new NotSupportedException($"Unable to multiply string {value} with {target}: type unrecognized");
            }
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case IIntegerConverter intTarget:
                    return new StringValue {value = DivideString(value, intTarget.ConvertToInteger(language))};
                case IFloatConverter floatTarget:
                    return new StringValue {value = DivideString(value, Mathf.RoundToInt(floatTarget.ConvertToFloat(language)))};
                case IStringConverter stringConverter:
                    throw new NotSupportedException($"Unable to divide string {value} with {stringConverter.ConvertToString(language)}: string cannot divide with string");
                case IBooleanConverter boolTarget:
                    return new StringValue {value = boolTarget.ConvertToBoolean(language) ? value : ""};
                default:
                    throw new NotSupportedException($"Unable to multiply {value} with {target}: type unrecognized");
            }
        }

        private static string DivideString(string source, int length) {
            if (length == source.Length) return source;
            if (length > source.Length) throw new NotSupportedException($"Unable to divide string {source}: length {length} unreached");
            var endIndex = Mathf.RoundToInt(source.Length / (float) length);
            if (endIndex > source.Length) {
                endIndex = source.Length;
            }
            return source.Substring(0, endIndex);
        }
    }
}