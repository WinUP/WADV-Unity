using System;
using System.Globalization;
using WADV.VisualNovel.Interoperation;
using UnityEngine;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个32位浮点数内存值</para>
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
    ///     <item><description>比较互操作器</description></item>
    ///     <item><description>真值比较互操作器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>子元素/特性支持</description></listheader>
    ///     <item><description>ToBoolean</description></item>
    ///     <item><description>ToNegative</description></item>
    ///     <item><description>ToInteger</description></item>
    ///     <item><description>ToString</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class FloatValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                              IEqualOperator, ICompareOperator, IPickChildOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// 尝试将可序列化值解析为32为浮点数值
        /// </summary>
        /// <param name="value">目标内存值</param>
        /// <returns></returns>
        public static float TryParse(SerializableValue value) {
            switch (value) {
                case IFloatConverter floatTarget:
                    return floatTarget.ConvertToFloat();
                case IIntegerConverter intTarget:
                    return intTarget.ConvertToInteger();
                case IStringConverter stringTarget:
                    var stringValue = stringTarget.ConvertToString();
                    if (int.TryParse(stringValue, out var intValue)) return intValue;
                    if (float.TryParse(stringValue, out var floatValue)) return floatValue;
                    throw new NotSupportedException($"Unable to convert {stringValue} to float: unsupported string format");
                case IBooleanConverter boolTarget:
                    return boolTarget.ConvertToBoolean() ? 1.0F : 0.0F;
                default:
                    throw new NotSupportedException($"Unable to convert {value} to float: unsupported format");
            }
        }
        
        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new FloatValue {Value = Value};
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return !Value.Equals(0.0F);
        }

        /// <inheritdoc />
        public bool ConvertToBoolean(string language) {
            return ConvertToBoolean();
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return Value;
        }

        /// <inheritdoc />
        public float ConvertToFloat(string language) {
            return ConvertToFloat();
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return Mathf.RoundToInt(Value);
        }

        /// <inheritdoc />
        public int ConvertToInteger(string language) {
            return ConvertToInteger();
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
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
        public SerializableValue PickChild(SerializableValue name) {
            if (!(name is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in float value with feature id {name}: only string feature name is accepted");
            var target = stringConverter.ConvertToString();
            switch (target) {
                case "ToBoolean":
                    return new BooleanValue {Value = ConvertToBoolean()};
                case "ToNegative":
                    return new FloatValue {Value = -Value};
                case "ToInteger":
                    return new IntegerValue {Value = ConvertToInteger()};
                case "ToString":
                    return new StringValue {Value = ConvertToString()};
                default:
                    throw new NotSupportedException($"Unable to get feature in float value: unsupported feature {target}");
            }
        }

        /// <inheritdoc />
        public SerializableValue PickChild(SerializableValue target, string language) {
            return PickChild(target);
        }

        /// <inheritdoc />
        public int CompareWith(SerializableValue target) {
            var value = Value - TryParse(target);
            return value.Equals(0.0F) ? 0 : value < 0 ? -1 : 1;
        }

        /// <inheritdoc />
        public int CompareWith(SerializableValue target, string language) {
            return CompareWith(target);
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target) {
            try {
                return Value.Equals(TryParse(target));
            } catch {
                return false;
            }
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target, string language) {
            return EqualsWith(target);
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return new FloatValue {Value = Value + TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target, string language) {
            return AddWith(target);
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            return new FloatValue {Value = Value - TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target, string language) {
            return SubtractWith(target);
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new FloatValue {Value = Value * TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target, string language) {
            return MultiplyWith(target);
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            return new FloatValue {Value = Value / TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target, string language) {
            return DivideWith(target);
        }
    }
}