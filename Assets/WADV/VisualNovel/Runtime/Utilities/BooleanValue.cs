using System;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="IBooleanConverter" />
    /// <inheritdoc cref="IFloatConverter" />
    /// <inheritdoc cref="IIntegerConverter" />
    /// <inheritdoc cref="IStringConverter" />
    /// <inheritdoc cref="IAddOperator" />
    /// <inheritdoc cref="IMultiplyOperator" />
    /// <inheritdoc cref="INegativeOperator" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <summary>
    /// <para>表示一个布尔可序列化内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>值复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>1 字节</description></item>
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
    ///     <item><description>乘法互操作器</description></item>
    ///     <item><description>取反互操作器</description></item>
    ///     <item><description>相等比较互操作器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class BooleanValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, IMultiplyOperator,
                                INegativeOperator, IEqualOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public bool value;

        /// <summary>
        /// 尝试将可序列化值解析为布尔值
        /// </summary>
        /// <param name="value">目标内存值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static bool TryParse(SerializableValue value, string language = TranslationManager.DefaultLanguage) {
            switch (value) {
                case IBooleanConverter booleanConverter:
                    return booleanConverter.ConvertToBoolean(language);
                case IFloatConverter floatConverter:
                    return !floatConverter.ConvertToFloat(language).Equals(0.0F);
                case IIntegerConverter integerConverter:
                    return integerConverter.ConvertToInteger(language) != 0;
                case IStringConverter stringConverter:
                    var upperValue = stringConverter.ConvertToString(language).ToUpper();
                    if (upperValue == "F" || upperValue == "FALSE") return false;
                    if (int.TryParse(upperValue, out var intValue) && intValue == 0) return false;
                    return !(float.TryParse(upperValue, out var floatValue) && floatValue.Equals(0.0F));
                default:
                    return value != null;
            }
        }

        public override SerializableValue Duplicate() {
            return new BooleanValue {value = value};
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return value;
        }

        public float ConvertToFloat(string language = TranslationManager.DefaultLanguage) {
            return value ? 1.0F : 0.0F;
        }

        public int ConvertToInteger(string language = TranslationManager.DefaultLanguage) {
            return value ? 1 : 0;
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return value ? "True" : "False";
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new BooleanValue {value = !value};
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return TryParse(target, language) == value;
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new BooleanValue {value = value || TryParse(target, language)};
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new BooleanValue {value = value && TryParse(target, language)};
        }
    }
}