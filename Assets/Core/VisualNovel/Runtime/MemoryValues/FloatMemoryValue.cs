using System;
using System.Globalization;
using Core.VisualNovel.Interoperation;
using UnityEngine;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个32位浮点数内存堆栈值</para>
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
    public class FloatMemoryValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
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
                case IIntegerConverter intTarget:
                    return intTarget.ConvertToInteger();
                case IFloatConverter floatTarget:
                    return floatTarget.ConvertToFloat();
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

        /// <inheritdoc />
        public override string ToString() {
            return $"FloatMemoryValue {{Value = {ConvertToString()}}}";
        }

        /// <inheritdoc />
        public SerializableValue PickChild(SerializableValue name) {
            if (!(name is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in float value with feature id {name}: only string feature name is accepted");
            var target = stringConverter.ConvertToString();
            switch (target) {
                case "ToBoolean":
                    return new BooleanMemoryValue {Value = ConvertToBoolean()};
                case "ToNegative":
                    return new FloatMemoryValue {Value = -Value};
                case "ToInteger":
                    return new IntegerMemoryValue {Value = ConvertToInteger()};
                case "ToString":
                    return new StringMemoryValue {Value = ConvertToString()};
                default:
                    throw new NotSupportedException($"Unable to get feature in float value: unsupported feature {target}");
            }
        }

        /// <inheritdoc />
        public int CompareWith(SerializableValue target) {
            var value = Value - TryParse(target);
            return value.Equals(0.0F) ? 0 : value < 0 ? -1 : 1;
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
        public SerializableValue AddWith(SerializableValue target) {
            return new FloatMemoryValue {Value = Value + TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            return new FloatMemoryValue {Value = Value - TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new FloatMemoryValue {Value = Value * TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            return new FloatMemoryValue {Value = Value / TryParse(target)};
        }
    }
}