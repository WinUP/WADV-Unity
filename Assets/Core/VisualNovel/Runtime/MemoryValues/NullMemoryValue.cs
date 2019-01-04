using System;
using Core.VisualNovel.Interoperation;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// 表示一个空内存堆栈值
    /// </summary>
    [Serializable]
    public class NullMemoryValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator {
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