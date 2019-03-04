using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;
using WADV.VisualNovel.Translation;

namespace WADV.Plugins.Unity {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="ISerializable" />
    /// <inheritdoc cref="IStringConverter" />
    /// <inheritdoc cref="IBooleanConverter" />
    /// <inheritdoc cref="IFloatConverter" />
    /// <inheritdoc cref="IIntegerConverter" />
    /// <inheritdoc cref="IAddOperator" />
    /// <inheritdoc cref="IMultiplyOperator" />
    /// <inheritdoc cref="ISubtractOperator" />
    /// <inheritdoc cref="IDivideOperator" />
    /// <inheritdoc cref="INegativeOperator" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <inheritdoc cref="IPickChildOperator" />
    /// <summary>
    /// <para>表示一个颜色值</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>值复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>4 字节</description></item>
    ///     <item><description>1 名称长度1的SerializationInfo项目</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>类型转换支持</description></listheader>
    ///     <item><description>布尔转换器</description></item>
    ///     <item><description>浮点转换器</description></item>
    ///     <item><description>整数转换器</description></item>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>加法互操作器</description></item>
    ///     <item><description>减法互操作器</description></item>
    ///     <item><description>乘法互操作器</description></item>
    ///     <item><description>除法互操作器</description></item>
    ///     <item><description>取反互操作器</description></item>
    ///     <item><description>相等比较互操作器</description></item>
    ///     <item><description>取子元素互操作器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>特性支持</description></listheader>
    ///     <item><description>R：获取红色分量（0-255）</description></item>
    ///     <item><description>G：获取绿色分量（0-255）</description></item>
    ///     <item><description>B：获取蓝色分量（0-255）</description></item>
    ///     <item><description>A/Alpha：获取透明度（0-255）</description></item>
    ///     <item><description>FloatR：获取红色分量（0-1）</description></item>
    ///     <item><description>FloatG：获取绿色分量（0-1）</description></item>
    ///     <item><description>FloatB：获取蓝色分量（0-1）</description></item>
    ///     <item><description>FloatA/FloatAlpha：获取透明度（0-1）</description></item>
    ///     <item><description>Hex：获以#开头的大写16进制颜色字符串</description></item>
    /// </list>
    /// </summary>
    /// <remarks>颜色值存储时按ARGB排列</remarks>
    [Serializable]
    public class ColorValue : SerializableValue, ISerializable, IStringConverter, IBooleanConverter, IFloatConverter, IIntegerConverter, IAddOperator, IMultiplyOperator,
                              ISubtractOperator, IDivideOperator, INegativeOperator, IEqualOperator, IPickChildOperator {
        /// <summary>
        /// 获取颜色结构
        /// </summary>
        public Color32 value;

        /// <summary>
        /// 获取透明度
        /// </summary>
        public float Alpha => value.a / 255.0F;

        /// <summary>
        /// 获取红色分量
        /// </summary>
        public float R => value.r / 255.0F;
        
        /// <summary>
        /// 获取绿色分量
        /// </summary>
        public float G => value.g / 255.0F;

        /// <summary>
        /// 获取蓝色分量
        /// </summary>
        public float B => value.b / 255.0F;

        /// <summary>
        /// 获取颜色ARGB数值
        /// </summary>
        public uint Argb => (uint) (value.a << 24) + (uint) (value.r << 16) + (uint) (value.g << 8) + value.b;
        
        /// <summary>
        /// 获取颜色RGBA数值
        /// </summary>
        public uint Rgba => (uint) (value.r << 24) + (uint) (value.g << 16) + (uint) (value.b << 8) + value.a;

        public ColorValue(uint argb) {
            var (r, g, b, a) = GetColorInteger(argb);
            value = new Color32(r, g, b, a);
        }

        public ColorValue(int argb) {
            var (r, g, b, a) = GetColorInteger(unchecked((uint) argb));
            value = new Color32(r, g, b, a);
        }

        public ColorValue(Color32 unityValue) {
            value = unityValue;
        }

        public ColorValue(byte r, byte g, byte b, byte a) {
            value = new Color32(r, g, b, a);
        }
        
        public ColorValue(float r, float g, float b, float a) {
            value = new Color32(ToByteColor(r), ToByteColor(g), ToByteColor(b), ToByteColor(a));
        }
        
        protected ColorValue(SerializationInfo info, StreamingContext context) {
            var (r, g, b, a) = GetColorInteger(info.GetUInt32("c"));
            value = new Color32(r, g, b, a);
        }
        
        public override SerializableValue Duplicate() {
            return new ColorValue(new Color32(value.r, value.g, value.b, value.a));
        }
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("c", Argb);
        }

        public static (byte R, byte G, byte B, byte A) GetColorInteger(uint color) {
            return ((byte) ((color & 0xFF0000) >> 16), (byte) ((color & 0xFF00) >> 8), (byte) (color & 0xFF), (byte) ((color & 0xFF000000) >> 24));
        }

        public static byte ToByteColor(float source) {
            return (byte) Mathf.Clamp(Mathf.RoundToInt(source * 255.0F), 0, 255);
        }

        public static float ToFloatColor(byte source) {
            return source / 255.0F;
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return $"#{value.a:X2}{value.r:X2}{value.g:X2}{value.b:X2}".ToUpper();
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return Argb != 0;
        }

        public float ConvertToFloat(string language = TranslationManager.DefaultLanguage) {
            return Argb;
        }

        public int ConvertToInteger(string language = TranslationManager.DefaultLanguage) {
            return unchecked((int) Argb);
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var (r, g, b, a) = GetColorProperties(target, language);
            return new ColorValue(r + R, g + G, b + B, a + Alpha);
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var (r, g, b, a) = GetColorProperties(target, language);
            return new ColorValue(r * R, g * G, b * B, a * Alpha);
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var (r, g, b, a) = GetColorProperties(target, language);
            return new ColorValue(r - R, g - G, b - B, a - Alpha);
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var (r, g, b, a) = GetColorProperties(target, language);
            return new ColorValue(r / R, g / G, b / B, a / Alpha);
        }
        
        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new ColorValue(1.0F - R, 1.0F - G, 1.0F - B, 1.0F - Alpha);
        }
        
        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var (r, g, b, a) = GetColorProperties(target, language);
            return r.Equals(R) && g.Equals(G) && b.Equals(B) && a.Equals(Alpha);
        }
        
        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is IStringConverter stringConverter)) throw new NotSupportedException($"Unable to pick child {target} from Vector2: only string is supported");
            var name = stringConverter.ConvertToString(language);
            switch (name) {
                case "R":
                    return new IntegerValue {value = value.r};
                case "G":
                    return new IntegerValue {value = value.g};
                case "B":
                    return new IntegerValue {value = value.b};
                case "A":
                case "Alpha":
                    return new IntegerValue {value = value.a};
                case "FloatR":
                    return new FloatValue {value = R};
                case "FloatG":
                    return new FloatValue {value = G};
                case "FloatB":
                    return new FloatValue {value = B};
                case "FloatA":
                case "FloatAlpha":
                    return new FloatValue {value = Alpha};
                case "Hex":
                    return new StringValue {value = ConvertToString(language)};
            }
            throw new NotSupportedException($"Unable to pick child {name} from Color: unrecognized command {name}");
        }

        private static (float R, float G, float B, float A) GetColorProperties(SerializableValue source, string language) {
            switch (source) {
                case ColorValue colorValue:
                    return (colorValue.R, colorValue.G, colorValue.B, colorValue.Alpha);
                default:
                    var defaultValue = FloatValue.TryParse(source, language);
                    if (defaultValue > 1.0F) {
                        defaultValue /= 255.0F;
                    }
                    return (defaultValue, defaultValue, defaultValue, defaultValue);
            }
        }
    }
}