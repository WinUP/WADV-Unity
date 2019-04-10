using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Intents;
using WADV.Plugins.Image.Effects;
using WADV.Translation;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="ISerializable" />
    /// <inheritdoc cref="IEqualOperator" />
    /// <inheritdoc cref="IStringConverter" />
    /// <summary>
    /// <para>表示一个适用于Graphic的特效</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>引用复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>8 字节</description></item>
    ///     <item><description>1 字符串</description></item>
    ///     <item><description>1 ObjectId</description></item>
    ///     <item><description>1 引用关联的System.Collections.Generic.Dictionary&lt;string, SerializableValue&gt;</description></item>
    ///     <item><description>4 名称长度1的SerializationInfo项目</description></item>
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
    public class EffectValue : SerializableValue, ISerializable, IEqualOperator, IStringConverter {
        /// <summary>
        /// 特效实例
        /// </summary>
        public GraphicEffect Effect { get; }
        
        /// <summary>
        /// 特效类型
        /// </summary>
        public string EffectType { get; }

        public EffectValue([NotNull] string type, [NotNull] GraphicEffect effect) {
            EffectType = type;
            Effect = effect;
        }
        
        protected EffectValue(SerializationInfo info, StreamingContext context) {
            EffectType = info.GetString("n");
            Effect = EffectPlugin.Create(
                EffectType,
                (Dictionary<string, SerializableValue>) info.GetValue("p", typeof(Dictionary<string, SerializableValue>)),
                info.GetSingle("d"),
                (EasingType) info.GetInt32("e")
            );
        }
        
        public override SerializableValue Clone() {
            return new EffectValue(EffectType, Effect);
        }

        public override Task BeforeRead(DumpRuntimeIntent.TaskLists tasks) {
            return Effect.Initialize();
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("n", EffectType);
            info.AddValue("d", Effect.Duration);
            info.AddValue("e", (int) Effect.EasingType);
            info.AddValue("p", Effect.Parameters);
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is EffectValue effectValue
                   && effectValue.EffectType == EffectType
                   && effectValue.Effect.Equals(Effect);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return EffectType;
        }
    }
}