using System;
using System.Globalization;
using WADV.VisualNovel.Interoperation;
using UnityEngine;
using WADV.Translation;

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
    /// <para>表示一个32位浮点数内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>值复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>4 字节</description></item>
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
    public class FloatValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                              INegativeOperator, ICompareOperator, IEqualOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public float value;

        /// <summary>
        /// 尝试将可序列化值解析为32为浮点数值
        /// </summary>
        /// <param name="value">目标内存值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static float TryParse(SerializableValue value, string language = TranslationManager.DefaultLanguage) {
            switch (value) {
                case BooleanValue booleanTarget:
                    return booleanTarget.ConvertToFloat(language);
                case FloatValue floatTarget:
                    return floatTarget.value;
                case IntegerValue integerTarget:
                    return integerTarget.ConvertToFloat(language);
                case NullValue nullTarget:
                    return nullTarget.ConvertToFloat(language);
                case StringValue stringTarget:
                    return stringTarget.ConvertToFloat(language);
                case TranslatableValue translatableTarget:
                    return translatableTarget.ConvertToFloat(language);
                case IFloatConverter floatTarget:
                    return floatTarget.ConvertToFloat(language);
                case IIntegerConverter intTarget:
                    return intTarget.ConvertToInteger(language);
                case IBooleanConverter boolTarget:
                    return boolTarget.ConvertToBoolean(language) ? 1.0F : 0.0F;
                case IStringConverter stringTarget:
                    var stringValue = stringTarget.ConvertToString(language);
                    if (int.TryParse(stringValue, out var intValue)) return intValue;
                    if (float.TryParse(stringValue, out var floatValue)) return floatValue;
                    throw new NotSupportedException($"Unable to convert {stringValue} to float: unsupported string format");
                default:
                    throw new NotSupportedException($"Unable to convert {value} to float: unsupported format");
            }
        }
        
        public override SerializableValue Clone() {
            return new FloatValue {value = value};
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return !value.Equals(0.0F);
        }

        public float ConvertToFloat(string language = TranslationManager.DefaultLanguage) {
            return value;
        }

        public int ConvertToInteger(string language = TranslationManager.DefaultLanguage) {
            return Mathf.RoundToInt(value);
        }
        
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new FloatValue {value = -value};
        }

        public int CompareWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var targetValue = value - TryParse(target, language);
            return targetValue.Equals(0.0F) ? 0 : targetValue < 0 ? -1 : 1;
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            try {
                return value.Equals(TryParse(target, language));
            } catch {
                return false;
            }
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new FloatValue {value = value + TryParse(target, language)};
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new FloatValue {value = value - TryParse(target, language)};
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new FloatValue {value = value * TryParse(target, language)};
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new FloatValue {value = value / TryParse(target, language)};
        }
    }
}