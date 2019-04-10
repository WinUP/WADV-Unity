using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Translation;
using WADV.VisualNovel.Interoperation;

namespace WADV.VisualNovel.Runtime.Utilities.Object {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="IPickChildOperator" />
    /// <inheritdoc cref="IStringConverter" />
    /// <summary>
    /// 表示一个对象内存值
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>索引使用值复制</description></item>
    ///     <item><description>索引目标使用引用复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>1 引用关联的System.Collections.Generic.Dictionary&lt;string, WriteBackReferenceValue&gt;</description></item>
    ///     <item><description>1 引用关联的System.Collections.Generic.Dictionary&lt;int, WriteBackReferenceValue&gt;</description></item>
    ///     <item><description>2 名称长度1的SerializationInfo项目</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>类型转换支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    ///<list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>取子元素互操作器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>子元素描述</description></listheader>
    ///     <item><description>Length: 获取数组部分的已知最大下标</description></item>
    ///     <item><description>ArrayCount: 获取数组部分的元素数目</description></item>
    ///     <item><description>KeyCount: 获取对象部分的元素数目</description></item>
    ///     <item><description>任意32位整数: 存取数组部分数据</description></item>
    ///     <item><description>任意非上述字符串: 存取对象部分数据</description></item>
    /// </list>
    /// </summary>
    /// <remarks>VNS对象分数组部分和对象部分，数组部分是线性存储序列，可使用32位整数作为下标存储可序列化值；对象部分是键值对存储序列，可使用字符串作为键存储可序列化值。</remarks>
    [Serializable]
    public class ObjectValue : SerializableValue, ISerializable, IPickChildOperator, IStringConverter {
        private Dictionary<string, WriteBackReferenceValue> _stringValues = new Dictionary<string, WriteBackReferenceValue>();
        private Dictionary<int, WriteBackReferenceValue> _integerValues = new Dictionary<int, WriteBackReferenceValue>();
        private readonly Action<WriteBackReferenceValue> _writeBackDelegate;
        
        public ObjectValue() {
            _writeBackDelegate = OnReferenceValueChanged;
        }
        
        protected ObjectValue(SerializationInfo info, StreamingContext context) {
            _stringValues = (Dictionary<string, WriteBackReferenceValue>) info.GetValue("o", typeof(Dictionary<string, WriteBackReferenceValue>));
            _integerValues = (Dictionary<int, WriteBackReferenceValue>) info.GetValue("a", typeof(Dictionary<int, WriteBackReferenceValue>));
            _writeBackDelegate = OnReferenceValueChanged;
        }

        public override SerializableValue Clone() {
            return new ObjectValue {
                _stringValues = _stringValues.Clone(),
                _integerValues = _integerValues.Clone()
            };
        }
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("a", _integerValues);
            info.AddValue("o", _stringValues);
        }

        /// <summary>
        /// 添加新项目
        /// </summary>
        /// <param name="name">项名</param>
        /// <param name="value">项值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        [NotNull]
        public ReferenceValue Add(SerializableValue name, SerializableValue value, string language = TranslationManager.DefaultLanguage) {
            void CheckKeyName(string key) {
                if (key == "Length" || key == "ArrayCount" || key == "KeyCount")
                    throw new NotSupportedException("Unable to set value in object: Length/ArrayCount/KeyCount is keyword");
            }
            WriteBackReferenceValue result;
            switch (name) {
                case IntegerValue integerMemoryValue:
                    if (_integerValues.ContainsKey(integerMemoryValue.value)) {
                        result = _integerValues[integerMemoryValue.value];
                    } else {
                        result = new WriteBackReferenceValue(_writeBackDelegate) {ReferenceTarget = value};
                        _integerValues.Add(integerMemoryValue.value, result);
                    }
                    break;
                case StringValue stringMemoryValue:
                    CheckKeyName(stringMemoryValue.value);
                    if (_stringValues.ContainsKey(stringMemoryValue.value)) {
                        result = _stringValues[stringMemoryValue.value];
                    } else {
                        result = new WriteBackReferenceValue(_writeBackDelegate) {ReferenceTarget = value};
                        _stringValues.Add(stringMemoryValue.value, result);
                    }
                    break;
                case IIntegerConverter integerConverter:
                    var integerValue = integerConverter.ConvertToInteger(language);
                    if (_integerValues.ContainsKey(integerValue)) {
                        result = _integerValues[integerValue];
                    } else {
                        result = new WriteBackReferenceValue(_writeBackDelegate) {ReferenceTarget = value};
                        _integerValues.Add(integerValue, result);
                    }
                    break;
                case IStringConverter stringConverter:
                    var stringValue = stringConverter.ConvertToString(language);
                    CheckKeyName(stringValue);
                    if (_stringValues.ContainsKey(stringValue)) {
                        result = _stringValues[stringValue];
                    } else {
                        result = new WriteBackReferenceValue(_writeBackDelegate) {ReferenceTarget = value};
                        _stringValues.Add(stringValue, result);
                    }
                    break;
                default:
                    var defaultStringValue = name.ToString();
                    CheckKeyName(defaultStringValue);
                    if (_stringValues.ContainsKey(defaultStringValue)) {
                        result = _stringValues[defaultStringValue];
                    } else {
                        result = new WriteBackReferenceValue(_writeBackDelegate) {ReferenceTarget = value};
                        _stringValues.Add(defaultStringValue, result);
                    }
                    break;
            }
            return result;
        }

        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            SerializableValue PickStringValue(string name) {
                switch (name) {
                    case "Length":
                        return new IntegerValue {value = _integerValues.Max(e => e.Key)};
                    case "ArrayCount":
                        return new IntegerValue {value = _integerValues.Count};
                    case "KeyCount":
                        return new IntegerValue {value = _stringValues.Count};
                    default:
                        return _stringValues.ContainsKey(name) ? _stringValues[name] : null;
                }
            }
            SerializableValue result;
            switch (target) {
                case IntegerValue integerMemoryValue:
                    result = _integerValues.ContainsKey(integerMemoryValue.value) ? _integerValues[integerMemoryValue.value] : null;
                    break;
                case StringValue stringMemoryValue:
                    result = PickStringValue(stringMemoryValue.value);
                    break;
                case IIntegerConverter integerConverter:
                    var integerValue = integerConverter.ConvertToInteger(language);
                    result = _integerValues.ContainsKey(integerValue) ? _integerValues[integerValue] : null;
                    break;
                case IStringConverter stringConverter:
                    result = PickStringValue(stringConverter.ConvertToString(language));
                    break;
                default:
                    result = PickStringValue(target.ToString());
                    break;
            }
            return result ?? Add(target, new NullValue(), language);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            var result = new StringBuilder($"[{string.Join(", ", _integerValues.Values)}");
            if (_stringValues.Any()) {
                result.Append($", {string.Join(", ", _stringValues.Select(e => $"{e.Key}={e.Value}"))}");
            }
            result.Append(']');
            return result.ToString();
        }

        private void OnReferenceValueChanged(WriteBackReferenceValue target) {
            if (target.ReferenceTarget != null && !(target.ReferenceTarget is NullValue)) return;
            foreach (var (key, _) in _integerValues.Where(e => e.Value == target).ToList()) {
                _integerValues.Remove(key);
            }
            foreach (var (key, _) in _stringValues.Where(e => e.Value == target).ToList()) {
                _stringValues.Remove(key);
            }
        }
    }
}