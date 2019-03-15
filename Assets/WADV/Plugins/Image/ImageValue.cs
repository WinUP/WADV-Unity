using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Plugins.Unity;
using WADV.Plugins.Vector;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Provider;
using WADV.VisualNovel.Translation;

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
    ///     <item><description>1 字符串</description></item>
    ///     <item><description>2 ObjectId</description></item>
    ///     <item><description>1 引用关联的WADV.Plugins.Vector.Rect2Value（当不是Rect2Value{0, 0, 1, 1}时）</description></item>
    ///     <item><description>1 引用关联的WADV.Plugins.Unity.ColorValue（当不是ColorValue{0, 0, 0, 255}时）</description></item>
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
                    _color = new ColorValue(0, 0, 0, 255);
                }
                return _color;
            }
            set => _color = value ?? new ColorValue(0, 0, 0, 255);
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
        /// 材质地址
        /// </summary>
        [CanBeNull] public string source;

        /// <summary>
        /// 材质
        /// </summary>
        [CanBeNull] public Texture2D texture;

        private RectValue _uv;
        private ColorValue _color;
        
        public ImageValue() { }
        
        protected ImageValue(SerializationInfo info, StreamingContext context) {
            _color = (ColorValue) info.GetValue("c", typeof(ColorValue));
            _uv = (RectValue) info.GetValue("r", typeof(RectValue));
            source = info.GetString("s");
        }

        /// <summary>
        /// 根据材质地址读取材质（不会重复读取）
        /// </summary>
        /// <param name="newSource">新的材质路径</param>
        /// <returns></returns>
        public async Task ReadTexture(string newSource = null) {
            if (string.IsNullOrEmpty(newSource)) {
                if (texture != null) return;
            } else {
                source = newSource;
            }
            texture = await ResourceManager.Load<Texture2D>(source);
        }
        
        public override SerializableValue Duplicate() {
            return new ImageValue {
                Color = (ColorValue) Color.Duplicate(),
                Uv = (RectValue) Uv.Duplicate(),
                source = source,
                texture = texture
            };
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("s", source);
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
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return source;
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is ImageValue imageValue && imageValue.source == source && Color.EqualsWith(imageValue.Color) && Uv.EqualsWith(imageValue.Uv);
        }
    }
}