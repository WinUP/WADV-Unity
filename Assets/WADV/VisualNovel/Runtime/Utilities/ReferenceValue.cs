using System;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个间接引用内存值</para>
    ///  <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>子元素/特性支持</description></listheader>
    ///     <item><description>ToString</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class ReferenceValue : SerializableValue, IStringConverter, IPickChildOperator {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public SerializableValue Value { get => _value;
            set {
                if (value == _value) return;
                if (IsConstant) throw new NotSupportedException("Cannot assign value to constant variable");
                _value = value ?? new NullValue();
            }
        }
        
        /// <summary>
        /// 获取或设置变量是否为常量
        /// </summary>
        public bool IsConstant { get; set; }

        private SerializableValue _value;

        public override SerializableValue Duplicate() {
            return new ReferenceValue {IsConstant = IsConstant, Value = Value.Duplicate()};
        }
        
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return Value is IStringConverter stringConverter ? stringConverter.ConvertToString(language) : Value.ToString();
        }

        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in variable with feature id {target}: only string feature name is accepted");
            var name = stringConverter.ConvertToString(language);
            switch (name) {
                case "ToString":
                    return new StringValue {Value = ConvertToString(language)};
                default:
                    throw new NotSupportedException($"Unable to get feature in variable: unsupported feature {name}");
            }
        }
    }
}