using System;
using Core.VisualNovel.Interoperation;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个空内存堆栈值</para>
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
    /// </summary>
    [Serializable]
    public class NullMemoryValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                                   IEqualOperator{
        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new NullMemoryValue();
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return false;
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return 0.0F;
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return 0;
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return "";
        }

        public override string ToString() {
            return $"NullMemoryValue {{}}";
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target) {
            return target is NullMemoryValue;
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return target.Duplicate();
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            return target is NullMemoryValue ? new NullMemoryValue() : throw new NotSupportedException("Unable to subtract null with any other value except null");
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new NullMemoryValue();
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            return target is NullMemoryValue ? new NullMemoryValue() : throw new NotSupportedException("Unable to divide null with any other value except null");
        }
    }
}