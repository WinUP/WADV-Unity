using System.Globalization;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个静态内存堆栈值
    /// </summary>
    public abstract class StaticMemoryValue : IMemoryValue {
        /// <summary>
        /// 获取此静态值对应的布尔值
        /// </summary>
        /// <returns></returns>
        public bool ToBoolean() {
            switch (this) {
                case StaticMemoryValue<string> stringMemoryValue:
                    var stringValue = stringMemoryValue.Value.ToLower();
                    return !string.IsNullOrEmpty(stringValue) && stringValue != "false" && stringValue != "f" && stringValue != "0" && stringValue != "0.0";
                case StaticMemoryValue<char> charMemoryValue:
                    var charValue = charMemoryValue.Value;
                    return charValue != 'f' && charValue != 'F' && charValue != '0' && charValue != '\0' && charValue != ' ';
                case StaticMemoryValue<bool> boolMemoryValue:
                    return boolMemoryValue.Value;
                case StaticMemoryValue<int> intMemoryValue:
                    return intMemoryValue.Value != 0;
                case StaticMemoryValue<long> longMemoryValue:
                    return longMemoryValue.Value != 0;
                case StaticMemoryValue<short> shortMemoryValue:
                    return shortMemoryValue.Value != 0;
                case StaticMemoryValue<sbyte> sbyteMemoryValue:
                    return sbyteMemoryValue.Value != 0;
                case StaticMemoryValue<byte> byteMemoryValue:
                    return byteMemoryValue.Value != 0;
                case StaticMemoryValue<ushort> ushortMemoryValue:
                    return ushortMemoryValue.Value != 0;
                case StaticMemoryValue<uint> uintMemoryValue:
                    return uintMemoryValue.Value != 0;
                case StaticMemoryValue<ulong> ulongMemoryValue:
                    return ulongMemoryValue.Value != 0;
                case StaticMemoryValue<float> floatMemoryValue:
                    return !floatMemoryValue.Value.Equals(0);
                case StaticMemoryValue<double> doubleMemoryValue:
                    return !doubleMemoryValue.Value.Equals(0);
                case StaticMemoryValue<decimal> decimalMemoryValue:
                    return !decimalMemoryValue.Value.Equals(0);
                default:
                    return (this as StaticMemoryValue<object>)?.Value != null;
            }
        }

        /// <summary>
        /// 获取此静态值对应的32位浮点数
        /// </summary>
        /// <returns></returns>
        public float ToFloat() {
            switch (this) {
                case StaticMemoryValue<string> stringMemoryValue:
                    return float.TryParse(stringMemoryValue.Value, out var floatValue) ? floatValue : 0.0F;
                case StaticMemoryValue<char> charMemoryValue:
                    switch (charMemoryValue.Value) {
                        case 'e':
                        case 'E':
                        case '1':
                            return 1.0F;
                        case '2':
                            return 2.0F;
                        case '3':
                            return 3.0F;
                        case '4':
                            return 4.0F;
                        case '5':
                            return 5.0F;
                        case '6':
                            return 6.0F;
                        case '7':
                            return 7.0F;
                        case '8':
                            return 8.0F;
                        case '9':
                            return 9.0F;
                        default:
                            return 0.0F;
                    }
                case StaticMemoryValue<bool> boolMemoryValue:
                    return boolMemoryValue.Value ? 1.0F : 0.0F;
                case StaticMemoryValue<int> intMemoryValue:
                    return intMemoryValue.Value;
                case StaticMemoryValue<long> longMemoryValue:
                    return longMemoryValue.Value;
                case StaticMemoryValue<short> shortMemoryValue:
                    return shortMemoryValue.Value;
                case StaticMemoryValue<sbyte> sbyteMemoryValue:
                    return sbyteMemoryValue.Value;
                case StaticMemoryValue<byte> byteMemoryValue:
                    return byteMemoryValue.Value;
                case StaticMemoryValue<ushort> ushortMemoryValue:
                    return ushortMemoryValue.Value;
                case StaticMemoryValue<uint> uintMemoryValue:
                    return uintMemoryValue.Value;
                case StaticMemoryValue<ulong> ulongMemoryValue:
                    return ulongMemoryValue.Value;
                case StaticMemoryValue<float> floatMemoryValue:
                    return floatMemoryValue.Value;
                case StaticMemoryValue<double> doubleMemoryValue:
                    return (float) doubleMemoryValue.Value;
                case StaticMemoryValue<decimal> decimalMemoryValue:
                    return (float) decimalMemoryValue.Value;
                default:
                    return (this as StaticMemoryValue<object>)?.Value != null ? 1.0F : 0.0F;
            }
        }

        /// <summary>
        /// 获取此静态值对应的32位整数
        /// </summary>
        /// <returns></returns>
        public int ToInteger() {
            switch (this) {
                case StaticMemoryValue<string> stringMemoryValue:
                    return int.TryParse(stringMemoryValue.Value, out var floatValue) ? floatValue : 0;
                case StaticMemoryValue<char> charMemoryValue:
                    switch (charMemoryValue.Value) {
                        case '1':
                            return 1;
                        case '2':
                            return 2;
                        case '3':
                            return 3;
                        case '4':
                            return 4;
                        case '5':
                            return 5;
                        case '6':
                            return 6;
                        case '7':
                            return 7;
                        case '8':
                            return 8;
                        case '9':
                            return 9;
                        default:
                            return 0;
                    }
                case StaticMemoryValue<bool> boolMemoryValue:
                    return boolMemoryValue.Value ? 1 : 0;
                case StaticMemoryValue<int> intMemoryValue:
                    return intMemoryValue.Value;
                case StaticMemoryValue<long> longMemoryValue:
                    return (int) longMemoryValue.Value;
                case StaticMemoryValue<short> shortMemoryValue:
                    return shortMemoryValue.Value;
                case StaticMemoryValue<sbyte> sbyteMemoryValue:
                    return sbyteMemoryValue.Value;
                case StaticMemoryValue<byte> byteMemoryValue:
                    return byteMemoryValue.Value;
                case StaticMemoryValue<ushort> ushortMemoryValue:
                    return ushortMemoryValue.Value;
                case StaticMemoryValue<uint> uintMemoryValue:
                    return (int) uintMemoryValue.Value;
                case StaticMemoryValue<ulong> ulongMemoryValue:
                    return (int) ulongMemoryValue.Value;
                case StaticMemoryValue<float> floatMemoryValue:
                    return (int) floatMemoryValue.Value;
                case StaticMemoryValue<double> doubleMemoryValue:
                    return (int) doubleMemoryValue.Value;
                case StaticMemoryValue<decimal> decimalMemoryValue:
                    return (int) decimalMemoryValue.Value;
                default:
                    return (this as StaticMemoryValue<object>)?.Value != null ? 1 : 0;
            }
        }

        /// <summary>
        /// 获取此静态值对应的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            switch (this) {
                case StaticMemoryValue<string> stringMemoryValue:
                    return stringMemoryValue.Value;
                case StaticMemoryValue<char> charMemoryValue:
                    return charMemoryValue.Value.ToString();
                case StaticMemoryValue<bool> boolMemoryValue:
                    return boolMemoryValue.Value ? "True" : "False";
                case StaticMemoryValue<int> intMemoryValue:
                    return intMemoryValue.Value.ToString();
                case StaticMemoryValue<long> longMemoryValue:
                    return longMemoryValue.Value.ToString();
                case StaticMemoryValue<short> shortMemoryValue:
                    return shortMemoryValue.Value.ToString();
                case StaticMemoryValue<sbyte> sbyteMemoryValue:
                    return sbyteMemoryValue.Value.ToString();
                case StaticMemoryValue<byte> byteMemoryValue:
                    return byteMemoryValue.Value.ToString();
                case StaticMemoryValue<ushort> ushortMemoryValue:
                    return ushortMemoryValue.Value.ToString();
                case StaticMemoryValue<uint> uintMemoryValue:
                    return uintMemoryValue.Value.ToString();
                case StaticMemoryValue<ulong> ulongMemoryValue:
                    return ulongMemoryValue.Value.ToString();
                case StaticMemoryValue<float> floatMemoryValue:
                    return floatMemoryValue.Value.ToString(CultureInfo.InvariantCulture);
                case StaticMemoryValue<double> doubleMemoryValue:
                    return doubleMemoryValue.Value.ToString(CultureInfo.InvariantCulture);
                case StaticMemoryValue<decimal> decimalMemoryValue:
                    return decimalMemoryValue.Value.ToString(CultureInfo.InvariantCulture);
                default:
                    return (this as StaticMemoryValue<object>)?.Value.ToString() ?? "";
            }
        }

        /// <inheritdoc />
        public abstract IMemoryValue Duplicate();
    }

    /// <inheritdoc />
    /// <summary>
    /// 表示一个静态内存堆栈值
    /// </summary>
    public class StaticMemoryValue<T> : StaticMemoryValue {
        /// <summary>
        /// 获取说设置值内容
        /// </summary>
        public T Value { get; set; }

        /// <inheritdoc />
        public override IMemoryValue Duplicate() {
            return new StaticMemoryValue<T> {Value = Value};
        }
    }
}