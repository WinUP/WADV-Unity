using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using JetBrains.Annotations;
using WADV.Reflection;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc />
    /// <summary>
    /// 为VNS提供对象支持（一定程度上可充当异构数组使用）
    /// </summary>
    [StaticRegistrationInfo("Object")]
    public class ObjectPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var result = new ObjectValue();
            foreach (var (key, value) in context.Parameters) {
                result.Add(key, value);
            }
            return Task.FromResult<SerializableValue>(result);
        }

        public bool OnRegister() => true;

        public bool OnUnregister(bool isReplace) => true;

        /// <inheritdoc cref="SerializableValue" />
        /// <summary>
        /// 表示一个对象内存值
        /// <para>VNS对象是键值对存储序列，可以使用32位浮点数、32位整数或字符串作为键值存储任意可序列化值并对可转换键值按上述优先级转换后查找元素</para>
        ///<list type="bullet">
        ///     <listheader><description>互操作支持</description></listheader>
        ///     <item><description>字符串转换器</description></item>
        ///     <item><description>取子元素互操作器</description></item>
        /// </list>
        /// <list type="bullet">
        ///     <listheader><description>子元素/特性支持</description></listheader>
        ///     <item><description>使用任意可识别布尔/数字/字符串作为键存储可序列化值</description></item>
        /// </list>
        /// </summary>
        [Serializable]
        public class ObjectValue : SerializableValue, IPickChildOperator, IStringConverter {
            private Dictionary<string, ReferenceValue> _stringValues = new Dictionary<string, ReferenceValue>();
            private Dictionary<float, ReferenceValue> _floatValues = new Dictionary<float, ReferenceValue>();
            private Dictionary<int, ReferenceValue> _integerValues = new Dictionary<int, ReferenceValue>();
            
            /// <inheritdoc />
            public override SerializableValue Duplicate() {
                return new ObjectValue {
                    _stringValues = _stringValues.Duplicate(),
                    _floatValues = _floatValues.Duplicate(),
                    _integerValues = _integerValues.Duplicate()
                };
            }

            [NotNull]
            public ReferenceValue Add(SerializableValue name, SerializableValue value) {
                ReferenceValue result;
                switch (name) {
                    case FloatValue floatMemoryValue:
                        if (_floatValues.ContainsKey(floatMemoryValue.Value)) {
                            result = _floatValues[floatMemoryValue.Value];
                        } else {
                            result = new ReferenceValue {Value = value};
                            _floatValues.Add(floatMemoryValue.Value, result);
                        }
                        break;
                    case IntegerValue integerMemoryValue:
                        if (_integerValues.ContainsKey(integerMemoryValue.Value)) {
                            result = _integerValues[integerMemoryValue.Value];
                        } else {
                            result = new ReferenceValue {Value = value};
                            _integerValues.Add(integerMemoryValue.Value, result);
                        }
                        break;
                    case StringValue stringMemoryValue:
                        if (_stringValues.ContainsKey(stringMemoryValue.Value)) {
                            result = _stringValues[stringMemoryValue.Value];
                        } else {
                            result = new ReferenceValue {Value = value};
                            _stringValues.Add(stringMemoryValue.Value, result);
                        }
                        break;
                    case IFloatConverter floatConverter:
                        var floatValue = floatConverter.ConvertToFloat();
                        if (_floatValues.ContainsKey(floatValue)) {
                            result = _floatValues[floatValue];
                        } else {
                            result = new ReferenceValue {Value = value};
                            _floatValues.Add(floatValue, result);
                        }
                        break;
                    case IIntegerConverter integerConverter:
                        var integerValue = integerConverter.ConvertToInteger();
                        if (_integerValues.ContainsKey(integerValue)) {
                            result = _integerValues[integerValue];
                        } else {
                            result = new ReferenceValue {Value = value};
                            _integerValues.Add(integerValue, result);
                        }
                        break;
                    case IStringConverter stringConverter:
                        var stringValue = stringConverter.ConvertToString();
                        if (_stringValues.ContainsKey(stringValue)) {
                            result = _stringValues[stringValue];
                        } else {
                            result = new ReferenceValue {Value = value};
                            _stringValues.Add(stringValue, result);
                        }
                        break;
                    default:
                        var defaultStringValue = name.ToString();
                        if (_stringValues.ContainsKey(defaultStringValue)) {
                            result = _stringValues[defaultStringValue];
                        } else {
                            result = new ReferenceValue {Value = value};
                            _stringValues.Add(defaultStringValue, result);
                        }
                        break;
                }
                return result;
            }

            /// <inheritdoc />
            [NotNull]
            public SerializableValue PickChild(SerializableValue name) {
                SerializableValue result;
                switch (name) {
                    case FloatValue floatMemoryValue:
                        result = _floatValues.ContainsKey(floatMemoryValue.Value) ? _floatValues[floatMemoryValue.Value] : null;
                        break;
                    case IntegerValue integerMemoryValue:
                        result = _integerValues.ContainsKey(integerMemoryValue.Value) ? _integerValues[integerMemoryValue.Value] : null;
                        break;
                    case StringValue stringMemoryValue:
                        result = _stringValues.ContainsKey(stringMemoryValue.Value) ? _stringValues[stringMemoryValue.Value] : null;
                        break;
                    case IFloatConverter floatConverter:
                        var floatValue = floatConverter.ConvertToFloat();
                        result = _floatValues.ContainsKey(floatValue) ? _floatValues[floatValue] : null;
                        break;
                    case IIntegerConverter integerConverter:
                        var integerValue = integerConverter.ConvertToInteger();
                        result = _integerValues.ContainsKey(integerValue) ? _integerValues[integerValue] : null;
                        break;
                    case IStringConverter stringConverter:
                        var stringValue = stringConverter.ConvertToString();
                        result = _stringValues.ContainsKey(stringValue) ? _stringValues[stringValue] : null;
                        break;
                    default:
                        var defaultStringValue = name.ToString();
                        result = _stringValues.ContainsKey(defaultStringValue) ? _stringValues[defaultStringValue] : null;
                        break;
                }
                return result ?? Add(name, new NullValue());
            }

            /// <inheritdoc />
            public SerializableValue PickChild(SerializableValue target, string language) {
                return PickChild(target);
            }

            /// <inheritdoc />
            public string ConvertToString() {
                var list = _floatValues.Values.ToList();
                list.AddRange(_integerValues.Values.ToList());
                list.AddRange(_stringValues.Values.ToList());
                return $"{string.Join(", ", list.Select(e => e.Value).Select(e => e is IStringConverter stringConverter ? stringConverter.ConvertToString() : e.ToString()))}";
            }
            
            /// <inheritdoc />
            public string ConvertToString(string language) {
                return ConvertToString();
            }
        }
    }
}