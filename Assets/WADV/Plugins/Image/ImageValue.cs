using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using WADV.Plugins.Unity;
using WADV.Translation;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="ISerializable" />
    /// <inheritdoc cref="IStringConverter" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <summary>
    /// <para>表示一个2D材质</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>对Graphic颜色使用值复制</description></item>
    ///     <item><description>对其他部分使用引用复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>3 ObjectId</description></item>
    ///     <item><description>1 引用关联的WADV.Plugins.Vector.Rect2Value（当不是Rect2Value{0, 0, 1, 1}时）</description></item>
    ///     <item><description>1 引用关联的WADV.Plugins.Unity.ColorValue（当不是ColorValue{0, 0, 0, 255}时）</description></item>
    ///     <item><description>1 引用关联的WADV.Plugins.Unity.Texture2DValue（当访问过Texture属性且其texture/source不均为空时）</description></item>
    ///     <item><description>3 名称长度1的SerializationInfo项目</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>类型转换支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>相等比较互操作器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class ImageValue : SerializableValue, ISerializable, IStringConverter, IEqualOperator {
        /// <summary>
        /// Graphic颜色
        /// </summary>
        public ColorValue Color {
            get {
                if (_color == null) {
                    _color = new ColorValue(255, 255, 255, 255);
                }
                return _color;
            }
            set => _color = value ?? new ColorValue(255, 255, 255, 255);
        }

        /// <summary>
        /// 材质的UV
        /// </summary>
        public RectValue Uv {
            get {
                if (_uv == null) {
                    _uv = new RectValue(0.0F, 0.0F, 1.0F, 1.0F);
                }
                return _uv;
            }
            set => _uv = value ?? new RectValue(0.0F, 0.0F, 1.0F, 1.0F);
        }

        /// <summary>
        /// 材质
        /// </summary>
        public Texture2DValue Texture {
            get {
                if (_texture == null) {
                    _texture = new Texture2DValue();
                }
                return _texture;
            }
            set {
                if (value != null && (_texture == null || !_texture.Equals(value))) {
                    _texture = value;
                }
            }
        }

        private RectValue _uv;
        private ColorValue _color;
        private Texture2DValue _texture;
        
        public ImageValue() { }
        
        protected ImageValue(SerializationInfo info, StreamingContext context) {
            _color = (ColorValue) info.GetValue("c", typeof(ColorValue));
            _uv = (RectValue) info.GetValue("r", typeof(RectValue));
            Texture = (Texture2DValue) info.GetValue("t", typeof(Texture2DValue));
        }
        
        public override SerializableValue Clone() {
            return new ImageValue {
                Color = (ColorValue) Color.Clone(),
                Uv = (RectValue) Uv.Clone(),
                Texture = Texture
            };
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            var rect = Uv.value;
            if (rect.x.Equals(0.0F) && rect.y.Equals(0.0F) && rect.width.Equals(1.0F) && rect.height.Equals(1.0F)) {
                info.AddValue("r", null, typeof(RectValue));
            } else {
                info.AddValue("r", Uv);
            }
            var color = Color.value;
            if (color.r == 0 && color.g == 0 && color.b == 0 && color.a == 255) {
                info.AddValue("c", null, typeof(ColorValue));
            } else {
                info.AddValue("c", Color);
            }
            if (_texture == null || _texture.texture == null && string.IsNullOrEmpty(_texture.source)) {
                info.AddValue("t", null, typeof(Texture2DValue));
            } else {
                info.AddValue("t", Texture);
            }
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return Texture.source;
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is ImageValue imageValue && imageValue.Texture.Equals(Texture) && Color.EqualsWith(imageValue.Color) && Uv.EqualsWith(imageValue.Uv);
        }
    }
}