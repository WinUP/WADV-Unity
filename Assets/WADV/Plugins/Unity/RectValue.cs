using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;
using WADV.Translation;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Unity {
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
    /// <para>表示一个二维矩形</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>值复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>16 字节</description></item>
    ///     <item><description>4 名称长度1的SerializationInfo项目</description></item>
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
    ///     <item><description>Width：获取宽度</description></item>
    ///     <item><description>Height：获取高度</description></item>
    ///     <item><description>CopyPosition：复制坐标</description></item>
    ///     <item><description>CopySize：复制大小</description></item>
    ///     <item><description>CopyArea：复制并重新计算新矩形坐标以保证长宽均为正值</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class RectValue : SerializableValue, ISerializable, IBooleanConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                              INegativeOperator, IPickChildOperator, ICompareOperator, IEqualOperator {
        public Rect value;

        private static (float X, float Y, float Width, float Height) NormalizeSize(float x, float y, float width, float height) {
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

        public RectValue() { }

        public RectValue(float x, float y, float width, float height) {
            value = new Rect {x = x, y = y, width = width, height = height};
        }

        protected RectValue(SerializationInfo info, StreamingContext context) {
            value = new Rect(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("w"), info.GetSingle("h"));
        }

        public override SerializableValue Duplicate() {
            return new RectValue(value.x, value.y, value.width, value.height);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("w", value.width);
            info.AddValue("h", value.height);
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return !value.width.Equals(0.0F) && !value.height.Equals(0.0F);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return $"Rect2 {{X={value.x.ToString(CultureInfo.InvariantCulture)}, Y={value.y.ToString(CultureInfo.InvariantCulture)}, Width={value.width.ToString(CultureInfo.InvariantCulture)}, Height={value.height.ToString(CultureInfo.InvariantCulture)}}}";
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case RectValue rectValue:
                    return new RectValue(value.x + rectValue.value.x, value.y + rectValue.value.y, value.width + rectValue.value.width, value.height + rectValue.value.height);
                case Vector3Value vector3Value:
                    return new RectValue(value.x + vector3Value.value.x, value.y + vector3Value.value.y, value.width, value.height);
                case Vector2Value vector2Value:
                    return new RectValue(value.x + vector2Value.value.x, value.y + vector2Value.value.y, value.width, value.height);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new RectValue(value.x + number, value.y + number, value.width, value.height);
            }
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case RectValue rectValue:
                    return new RectValue(value.x - rectValue.value.x, value.y - rectValue.value.y, value.width - rectValue.value.width, value.height - rectValue.value.height);
                case Vector3Value vector3Value:
                    return new RectValue(value.x - vector3Value.value.x, value.y - vector3Value.value.y, value.width, value.height);
                case Vector2Value vector2Value:
                    return new RectValue(value.x - vector2Value.value.x, value.y - vector2Value.value.y, value.width, value.height);
                default:
                    var number = FloatValue.TryParse(target, language);
                    return new RectValue(value.x - number, value.y - number, value.width, value.height);
            }
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var number = FloatValue.TryParse(target, language);
            return new RectValue(value.x, value.y, value.width * number, value.height * number);
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var number = FloatValue.TryParse(target, language);
            return new RectValue(value.x, value.y, value.width / number, value.height / number);
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new RectValue(-value.x, -value.y, -value.width, -value.height);
        }

        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is IStringConverter stringConverter)) throw new NotSupportedException($"Unable to pick child {target} from Rect2: only string is supported");
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
                case "CopyArea":
                    var (x, y, width, height) = NormalizeSize(value.x, value.y, value.width, value.height);
                    return new RectValue(x, y, width, height);
                default:
                    throw new NotSupportedException($"Unable to pick child {name} from Rect2: unrecognized command {name}");
            }
        }

        public int CompareWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            switch (target) {
                case RectValue rectValue:
                    var result = value.width * value.height - rectValue.value.width * rectValue.value.height;
                    return result > 0 ? 1 : result < 0 ? -1 : 0;
                default:
                    throw new NotSupportedException($"Unable to compare Rect2 with {target}: unsupported type");
            }
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is RectValue rectValue && value.Equals(rectValue.value);
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