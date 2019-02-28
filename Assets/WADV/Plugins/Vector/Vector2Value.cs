using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
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
    /// <para>表示一个二维向量</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>值复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>8 字节</description></item>
    ///     <item><description>2 名称长度1的SerializationInfo项目</description></item>
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
    /// </list>
    /// </summary>
    [Serializable]
    public class Vector2Value : SerializableValue, ISerializable, IBooleanConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                                INegativeOperator, IPickChildOperator, ICompareOperator, IEqualOperator {
        public Vector2 value;
        
        public Vector2Value() { }

        public Vector2Value(float x, float y) {
            value = new Vector2(x, y);
        }
        
        protected Vector2Value(SerializationInfo info, StreamingContext context) {
            value = new Vector2(info.GetSingle("x"), info.GetSingle("y"));
        }
        
        public override SerializableValue Duplicate() {
            return new Vector2Value(value.x, value.y);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return !value.x.Equals(0.0F) && !value.y.Equals(0.0F);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return $"Vector2 {{X={value.x.ToString(CultureInfo.InvariantCulture)}, Y={value.y.ToString(CultureInfo.InvariantCulture)}}}";
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is IStringConverter stringConverter)) throw new NotSupportedException($"Unable to pick child {target} from Vector2: only string is supported");
            var name = stringConverter.ConvertToString(language);
            switch (name) {
                case "X":
                    return new WriteBackReferenceValue(new FloatValue {value = value.x}, WriteXBack);
                case "Y":
                    return new WriteBackReferenceValue(new FloatValue {value = value.y}, WriteYBack);
                default:
                    throw new NotSupportedException($"Unable to pick child {name} from Vector2: unrecognized command {name}");
            }
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector2Value(value.x + vector3Value.value.x, value.y + vector3Value.value.y);
                case Vector2Value vector2Value:
                    return new Vector2Value(value.x + vector2Value.value.x, value.y + vector2Value.value.y);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new Vector2Value(value.x + number, value.y + number);
            }
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector2Value(value.x - vector3Value.value.x, value.y - vector3Value.value.y);
                case Vector2Value vector2Value:
                    return new Vector2Value(value.x - vector2Value.value.x, value.y - vector2Value.value.y);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new Vector2Value(value.x - number, value.y - number);
            }
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector2Value(value.x * vector3Value.value.x, value.y * vector3Value.value.y);
                case Vector2Value vector2Value:
                    return new Vector2Value(value.x * vector2Value.value.x, value.y * vector2Value.value.y);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new Vector2Value(value.x * number, value.y * number);
            }
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector2Value(value.x / vector3Value.value.x, value.y / vector3Value.value.y);
                case Vector2Value vector2Value:
                    return new Vector2Value(value.x / vector2Value.value.x, value.y / vector2Value.value.y);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new Vector2Value(value.x / number, value.y / number);
            }
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new Vector2Value(-value.x, -value.y);
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
            return target is Vector2Value vector2 && value.Equals(vector2.value) ||
                   target is Vector3Value vector3 && vector3.value.z.Equals(0.0F) && value.Equals(new Vector2(vector3.value.x, vector3.value.y));
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
    }
}