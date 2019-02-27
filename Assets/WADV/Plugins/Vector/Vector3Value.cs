using System;
using System.Globalization;
using System.Runtime.Serialization;
using UnityEngine;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;
using WADV.VisualNovel.Translation;

namespace WADV.Plugins.Vector {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="IBooleanConverter" />
    /// <inheritdoc cref="IStringConverter" />
    /// <inheritdoc cref="IAddOperator" />
    /// <inheritdoc cref="ISubtractOperator" />
    /// <inheritdoc cref="IMultiplyOperator" />
    /// <inheritdoc cref="IDivideOperator" />
    /// <inheritdoc cref="INegativeOperator" />
    /// <inheritdoc cref="IPickChildOperator" />
    /// <inheritdoc cref="ICompareOperator" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <summary>
    /// <para>表示一个三维向量</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>值复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>12 字节</description></item>
    ///     <item><description>3 名称长度1的SerializationInfo项目</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>类型转换支持</description></listheader>
    ///     <item><description>布尔转换器</description></item>
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
    /// <list type="bullet">
    ///     <listheader><description>特性支持</description></listheader>
    ///     <item><description>X：获取X分量</description></item>
    ///     <item><description>Y：获取Y分量</description></item>
    ///     <item><description>Z：获取Z分量</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class Vector3Value : SerializableValue, ISerializable, IBooleanConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                                INegativeOperator, IPickChildOperator, ICompareOperator, IEqualOperator {
        public Vector3 value;
        
        public Vector3Value() { }
        
        public Vector3Value(float x, float y, float z) {
            value = new Vector3(x, y, z);
        }
        
        protected Vector3Value(SerializationInfo info, StreamingContext context) {
            value = new Vector3(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"));
        }
        
        public override SerializableValue Duplicate() {
            return new Vector2Value {value = new Vector3(value.x, value.y, value.z)};
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("z", value.z);
        }
        
        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return !value.x.Equals(0.0F) && !value.y.Equals(0.0F) && !value.z.Equals(0.0F);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return $"Vector3 {{X={value.x.ToString(CultureInfo.InvariantCulture)}, Y={value.y.ToString(CultureInfo.InvariantCulture)}, Z={value.z.ToString(CultureInfo.InvariantCulture)}}}";
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is IStringConverter stringConverter)) throw new NotSupportedException($"Unable to pick child {target} from Vector3: only string is supported");
            var name = stringConverter.ConvertToString(language);
            switch (name) {
                case "X":
                    return new WriteBackReferenceValue(new FloatValue {value = value.x}, WriteXBack);
                case "Y":
                    return new WriteBackReferenceValue(new FloatValue {value = value.y}, WriteYBack);
                case "Z":
                    return new WriteBackReferenceValue(new FloatValue {value = value.z}, WriteZBack);
                default:
                    throw new NotSupportedException($"Unable to pick child {name} from Vector2: unrecognized command {name}");
            }
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector3Value(value.x + vector3Value.value.x, value.y + vector3Value.value.y, value.z + vector3Value.value.z);
                case Vector2Value vector2Value:
                    return new Vector3Value(value.x + vector2Value.value.x, value.y + vector2Value.value.y, value.z);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new Vector3Value(value.x + number, value.y + number, value.z + number);
            }
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector3Value(value.x - vector3Value.value.x, value.y - vector3Value.value.y, value.z - vector3Value.value.z);
                case Vector2Value vector2Value:
                    return new Vector3Value(value.x - vector2Value.value.x, value.y - vector2Value.value.y, value.z);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new Vector3Value(value.x - number, value.y - number, value.z - number);
            }
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector3Value(
                        value.y * vector3Value.value.z - value.z * vector3Value.value.y,
                        value.z * vector3Value.value.x - value.x * vector3Value.value.z,
                        value.x * vector3Value.value.y - value.y * vector3Value.value.x);
                case Vector2Value vector2Value:
                    return new Vector3Value(
                        -value.z * vector2Value.value.y,
                        value.z * vector2Value.value.x,
                        value.x * vector2Value.value.y - value.y * vector2Value.value.x);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new Vector3Value(value.x * number, value.y * number, value.z * number);
            }
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var number = FloatValue.TryParse(target, language);
            return new Vector3Value(value.x / number, value.y / number, value.z / number);
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new Vector3Value(-value.x, -value.y, -value.z);
        }

        public int CompareWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            float result;
            switch (target) {
                case Vector3Value vector3Value:
                    result = value.sqrMagnitude - vector3Value.value.sqrMagnitude;
                    break;
                case Vector2Value vector2Value:
                    result = value.sqrMagnitude - vector2Value.value.sqrMagnitude;
                    break;
                default:
                    result = value.magnitude - FloatValue.TryParse(target, language);
                    break;
            }
            return result > 0 ? 1 : result < 0 ? -1 : 0;
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is Vector3Value vector3 && value.Equals(vector3.value) ||
                   target is Vector2Value vector2 && value.z.Equals(0.0F) && new Vector2(value.x, value.y).Equals(vector2.value);
        }
        
        private void WriteXBack(WriteBackReferenceValue target) {
            if (target.ReferenceTarget is IFloatConverter floatConverter) {
                value.x = floatConverter.ConvertToFloat();
            }
        }
        
        private void WriteYBack(WriteBackReferenceValue target) {
            if (target.ReferenceTarget is IFloatConverter floatConverter) {
                value.y = floatConverter.ConvertToFloat();
            }
        }
        
        private void WriteZBack(WriteBackReferenceValue target) {
            if (target.ReferenceTarget is IFloatConverter floatConverter) {
                value.z = floatConverter.ConvertToFloat();
            }
        }
    }
}