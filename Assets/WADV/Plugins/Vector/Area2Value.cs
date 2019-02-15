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
    /// <para>表示一个二维区域</para>
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
    ///     <item><description>Width：获取宽度</description></item>
    ///     <item><description>Height：获取高度</description></item>
    ///     <item><description>CopyPosition：复制坐标</description></item>
    ///     <item><description>CopySize：复制大小</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class Area2Value : SerializableValue, ISerializable, IBooleanConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                              INegativeOperator, IPickChildOperator, ICompareOperator, IEqualOperator {
        public static (float X, float Y, float Width, float Height) NormalizeSize(float x, float y, float width, float height) {
            if (width < 0) {
                x -= width;
                width = -width;
            }
            if (height < 0) {
                y -= height;
                height = -height;
            }
            return (x, y, width, height);
        }
        
        public Rect value;
        
        public Area2Value() { }

        public Area2Value(float x, float y, float width, float height) {
            (x, y, width, height) = NormalizeSize(x, y, width, height);
            value = new Rect {x = x, y = y, width = width, height = height};
        }
        
        protected Area2Value(SerializationInfo info, StreamingContext context) {
            value = new Rect(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("width"), info.GetSingle("height"));
        }
        
        public override SerializableValue Duplicate() {
            return new Area2Value(value.x, value.y, value.width, value.height);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("width", value.width);
            info.AddValue("height", value.height);
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return !value.width.Equals(0.0F) && !value.height.Equals(0.0F);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return $"Area2 {{X={value.x.ToString(CultureInfo.InvariantCulture)}, Y={value.y.ToString(CultureInfo.InvariantCulture)}, Width={value.width.ToString(CultureInfo.InvariantCulture)}, Height={value.height.ToString(CultureInfo.InvariantCulture)}}}";
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Rect2Value rectValue:
                    return new Area2Value(value.x + rectValue.value.x, value.y + rectValue.value.y, value.width + rectValue.value.width, value.height + rectValue.value.height);
                case Area2Value areaValue:
                    return new Area2Value(value.x + areaValue.value.x, value.y + areaValue.value.y, value.width + areaValue.value.width, value.height + areaValue.value.height);
                case Vector3Value vector3Value:
                    return new Area2Value(value.x + vector3Value.value.x, value.y + vector3Value.value.y, value.width, value.height);
                case Vector2Value vector2Value:
                    return new Area2Value(value.x + vector2Value.value.x, value.y + vector2Value.value.y, value.width, value.height);
                default:
                    var number = FloatValue.TryParse(target);
                    return new Area2Value(value.x + number, value.y + number, value.width, value.height);
            }
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Rect2Value rectValue:
                    return new Area2Value(value.x + rectValue.value.x, value.y + rectValue.value.y, value.width + rectValue.value.width, value.height + rectValue.value.height);
                case Area2Value rectValue:
                    return new Area2Value(value.x - rectValue.value.x, value.y - rectValue.value.y, value.width - rectValue.value.width, value.height - rectValue.value.height);
                case Vector3Value vector3Value:
                    return new Area2Value(value.x - vector3Value.value.x, value.y - vector3Value.value.y, value.width, value.height);
                case Vector2Value vector2Value:
                    return new Area2Value(value.x - vector2Value.value.x, value.y - vector2Value.value.y, value.width, value.height);
                default:
                    var number = FloatValue.TryParse(target);
                    return new Area2Value(value.x - number, value.y - number, value.width, value.height);
            }
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var number = FloatValue.TryParse(target);
            return new Area2Value(value.x, value.y, value.width * number, value.height * number);
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var number = FloatValue.TryParse(target);
            return new Area2Value(value.x, value.y, value.width / number, value.height / number);
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new Area2Value(-value.x, -value.y, -value.width, -value.height);
        }

        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is IStringConverter stringConverter)) throw new NotSupportedException($"Unable to pick child {target} from Area2: only string is supported");
            var name = stringConverter.ConvertToString(language);
            switch (name) {
                case "X":
                    return new WriteBackReferenceValue(new FloatValue {value = value.x}, WriteXBack);
                case "Y":
                    return new WriteBackReferenceValue(new FloatValue {value = value.y}, WriteYBack);
                case "Width":
                    return new WriteBackReferenceValue(new FloatValue {value = value.width}, WriteWidthBack);
                case "Height":
                    return new WriteBackReferenceValue(new FloatValue {value = value.height}, WriteHeightBack);
                case "CopyPosition":
                    return new Vector2Value(value.x, value.y);
                case "CopySize":
                    return new Vector2Value(value.width, value.height);
                default:
                    throw new NotSupportedException($"Unable to pick child {name} from Area2: unrecognized command {name}");
            }
        }

        public int CompareWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case Area2Value rectValue:
                    var result = value.width * value.height - rectValue.value.width * rectValue.value.height;
                    return result > 0 ? 1 : result < 0 ? -1 : 0;
                default:
                    throw new NotSupportedException($"Unable to compare Area2 with {target}: unsupported type");
            }
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is Area2Value rectValue && value.Equals(rectValue.value);
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
        
        private void WriteWidthBack(WriteBackReferenceValue target) {
            if (target.ReferenceTarget is IFloatConverter floatConverter) {
                value.width = floatConverter.ConvertToFloat();
            }
        }
        
        private void WriteHeightBack(WriteBackReferenceValue target) {
            if (target.ReferenceTarget is IFloatConverter floatConverter) {
                value.height = floatConverter.ConvertToFloat();
            }
        }
    }
}