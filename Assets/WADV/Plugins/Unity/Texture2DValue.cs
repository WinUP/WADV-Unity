using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Resource;
using WADV.Translation;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Unity {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="ISerializable" />
    /// <inheritdoc cref="IStringConverter" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <summary>
    /// <para>表示一个2D材质</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>引用复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>1 字符串</description></item>
    ///     <item><description>1 名称长度1的SerializationInfo项目</description></item>
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
    public class Texture2DValue : SerializableValue, ISerializable, IStringConverter, IEqualOperator {
        /// <summary>
        /// 材质
        /// </summary>
        [CanBeNull] public Texture2D texture;

        /// <summary>
        /// 材质地址
        /// </summary>
        [CanBeNull] public string source;
        
        public Texture2DValue() { }
        
        protected Texture2DValue(SerializationInfo info, StreamingContext context) {
            source = info.GetString("s");
        }
        
        public override SerializableValue Clone() {
            return new Texture2DValue {
                texture = texture,
                source = source
            };
        }
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("s", source);
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
            if (string.IsNullOrEmpty(source)) return;
            texture = await ResourceManager.Load<Texture2D>(source);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return source;
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is Texture2DValue texture2DValue && texture2DValue.source == source;
        }
    }
}