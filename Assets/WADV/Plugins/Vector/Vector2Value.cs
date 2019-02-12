using System;
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
    /// <inheritdoc cref="ICompareOperator" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <summary>
    /// <para>表示一个32位整数内存值</para>
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
    /// </summary>
    [Serializable]
    public class Vector2Value : SerializableValue, ISerializable, IBooleanConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                                INegativeOperator, ICompareOperator, IEqualOperator {
        public Vector2 Value { get; set; }
        
        public Vector2Value() { }

        public Vector2Value(float x, float y) {
            Value = new Vector2(x, y);
        }
        
        protected Vector2Value(SerializationInfo info, StreamingContext context) {
            Value = new Vector2(info.GetSingle("x"), info.GetSingle("y"));
        }
        
        public override SerializableValue Duplicate() {
            return new Vector2Value(Value.x, Value.y);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", Value.x);
            info.AddValue("y", Value.y);
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return !Value.x.Equals(0.0F) && !Value.y.Equals(0.0F);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return $"Vector2 {{X={Value.x}, Y={Value.y}}}";
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector2Value(Value.x + vector3Value.Value.x, Value.y + vector3Value.Value.y);
                case Vector2Value vector2Value:
                    return new Vector2Value(Value.x + vector2Value.Value.x, Value.y + vector2Value.Value.y);
                default:
                    var number = FloatValue.TryParse(target);
                    return new Vector2Value(Value.x + number, Value.y + number);
            }
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector2Value(Value.x - vector3Value.Value.x, Value.y - vector3Value.Value.y);
                case Vector2Value vector2Value:
                    return new Vector2Value(Value.x - vector2Value.Value.x, Value.y - vector2Value.Value.y);
                default:
                    var number = FloatValue.TryParse(target);
                    return new Vector2Value(Value.x - number, Value.y - number);
            }
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector2Value(Value.x * vector3Value.Value.x, Value.y * vector3Value.Value.y);
                case Vector2Value vector2Value:
                    return new Vector2Value(Value.x * vector2Value.Value.x, Value.y * vector2Value.Value.y);
                default:
                    var number = FloatValue.TryParse(target);
                    return new Vector2Value(Value.x * number, Value.y * number);
            }
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Vector3Value vector3Value:
                    return new Vector2Value(Value.x / vector3Value.Value.x, Value.y / vector3Value.Value.y);
                case Vector2Value vector2Value:
                    return new Vector2Value(Value.x / vector2Value.Value.x, Value.y / vector2Value.Value.y);
                default:
                    var number = FloatValue.TryParse(target);
                    return new Vector2Value(Value.x / number, Value.y / number);
            }
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new Vector2Value(-Value.x, -Value.y);
        }

        public int CompareWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            float result;
            switch (target) {
                case Vector3Value vector3Value:
                    result = Value.sqrMagnitude - vector3Value.Value.sqrMagnitude;
                    break;
                case Vector2Value vector2Value:
                    result = Value.sqrMagnitude - vector2Value.Value.sqrMagnitude;
                    break;
                default:
                    result = Value.magnitude - FloatValue.TryParse(target);
                    break;
            }
            return result > 0 ? 1 : result < 0 ? -1 : 0;
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is Vector2Value vector2 && Value.Equals(vector2.Value) ||
                   target is Vector3Value vector3 && vector3.Value.z.Equals(0.0F) && Value.Equals(new Vector2(vector3.Value.x, vector3.Value.y));
        }
    }
}