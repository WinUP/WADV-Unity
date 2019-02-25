using System;
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
    /// <inheritdoc cref="ICompareOperator" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <summary>
    /// <para>表示一个32位整数内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>值复制</description></item>
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
    ///     <item><description>大小比较互操作器</description></item>
    ///     <item><description>相等比较互操作器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class IntegerValue : SerializableValue, IBooleanConverter, IIntegerConverter, IFloatConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                                INegativeOperator, ICompareOperator, IEqualOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public int value;

        /// <summary>
        /// 尝试将可序列化值解析为32位整数值
        /// </summary>
        /// <param name="value">目标内存值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static int TryParse(SerializableValue value, string language = TranslationManager.DefaultLanguage) {
            switch (value) {
                case IIntegerConverter intTarget:
                    return intTarget.ConvertToInteger(language);
                case IFloatConverter floatTarget:
                    return Mathf.RoundToInt(floatTarget.ConvertToFloat(language));
                case IStringConverter stringTarget:
                    var stringValue = stringTarget.ConvertToString(language);
                    if (int.TryParse(stringValue, out var intValue)) return intValue;
                    if (float.TryParse(stringValue, out var floatValue)) return Mathf.RoundToInt(floatValue);
                    throw new NotSupportedException($"Unable to convert {stringValue} to integer: unsupported string format");
                case IBooleanConverter boolTarget:
                    return boolTarget.ConvertToBoolean(language) ? 1 : 0;
                default:
                    throw new NotSupportedException($"Unable to convert {value} to integer: unsupported format");
            }
        }

        public override SerializableValue Duplicate() {
            return new IntegerValue {value = value};
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return value != 0;
        }

        public int ConvertToInteger(string language = TranslationManager.DefaultLanguage) {
            return value;
        }

        public float ConvertToFloat(string language = TranslationManager.DefaultLanguage) {
            return value;
        }
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return value.ToString();
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new IntegerValue {value = -value};
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            try {
                return value == TryParse(target, language);
            } catch {
                return false;
            }
        }

        public int CompareWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var targetValue = value - FloatValue.TryParse(target, language);
            return targetValue.Equals(0.0F) ? 0 : targetValue < 0 ? -1 : 1;
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new IntegerValue {value = value + TryParse(target, language)};
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new IntegerValue {value = value - TryParse(target, language)};
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new IntegerValue {value = value * TryParse(target, language)};
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new IntegerValue {value = value / TryParse(target, language)};
        }
    }
}