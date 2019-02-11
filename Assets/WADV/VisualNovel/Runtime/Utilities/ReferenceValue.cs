using System;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="IStringConverter" />
    /// <summary>
    /// <para>表示一个间接引用内存值</para>
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
        public SerializableValue Value { get => _value;
            set {
                if (value == _value) return;
                if (IsConstant) throw new NotSupportedException("Cannot assign value to constant variable");
                _value = value ?? new NullValue();
                OnValueChanged?.Invoke(this, _value);
            }
        }
        
        /// <summary>
        /// 获取或设置变量是否为常量
        /// </summary>
        public bool IsConstant { get; set; }

        private SerializableValue _value;

        /// <summary>
        /// 值变化后通知事件
        /// </summary>
        public event Action<ReferenceValue, SerializableValue> OnValueChanged; 

        public override SerializableValue Duplicate() {
            return new ReferenceValue {IsConstant = IsConstant, Value = Value.Duplicate()};
        }
        
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return Value is IStringConverter stringConverter ? stringConverter.ConvertToString(language) : Value.ToString();
        }
    }
}