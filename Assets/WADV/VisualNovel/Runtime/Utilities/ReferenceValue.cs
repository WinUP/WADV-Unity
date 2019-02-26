using System;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="IStringConverter" />
    /// <summary>
    /// <para>表示一个间接引用内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>外部使用引用复制</description></item>
    ///     <item><description>被引用对象的复制方式取决于其本身</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>1 字节</description></item>
    ///     <item><description>1 ObjectId</description></item>
    ///     <item><description>1 引用关联的WADV.VisualNovel.Interoperation.SerializableValue</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>类型转换支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class ReferenceValue : SerializableValue, IStringConverter {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public virtual SerializableValue ReferenceTarget { get => _referenceTarget;
            set {
                if (value == _referenceTarget) return;
                if (IsConstant) throw new NotSupportedException($"Unable to change variable value {value}: cannot assign value to constant variable");
                _referenceTarget = value ?? new NullValue();
            }
        }
        
        /// <summary>
        /// 获取或设置变量是否为常量
        /// </summary>
        public bool IsConstant { get; set; }

        private SerializableValue _referenceTarget;
        
        public ReferenceValue() { }

        public ReferenceValue(SerializableValue referenceTarget) {
            _referenceTarget = referenceTarget;
        }

        public override SerializableValue Duplicate() {
            return new ReferenceValue {IsConstant = IsConstant, ReferenceTarget = ReferenceTarget.Duplicate()};
        }
        
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return ReferenceTarget is IStringConverter stringConverter ? stringConverter.ConvertToString(language) : ReferenceTarget.ToString();
        }
    }
}