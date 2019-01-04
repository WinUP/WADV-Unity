using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Extensions;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;
using Core.VisualNovel.Runtime.MemoryValues;
using JetBrains.Annotations;

namespace Core.VisualNovelPlugins {
    /// <inheritdoc />
    /// <summary>
    /// 为VNS提供对象支持（一定程度上可充当异构数组使用）
    /// </summary>
    public class ObjectPlugin : VisualNovelPlugin {
        public ObjectPlugin() : base("Object") { }
        
        public override Task<SerializableValue> Execute(ScriptRuntime context, IDictionary<SerializableValue, SerializableValue> parameters) {
            var result = new ObjectDelegate();
            foreach (var (key, value) in parameters) {
                result.Add(key, value);
            }
            return Task.FromResult<SerializableValue>(result);
        }

        /// <inheritdoc cref="SerializableValue" />
        /// <summary>
        /// 表示一个VNS对象
        /// <para>VNS对象是一个键值对存储序列，可以使用32位浮点数、32位整数或字符串作为键值存储任意SerializableValue并对可转换键值按上述优先级查找元素</para>
        /// </summary>
        [Serializable]
        private class ObjectDelegate : SerializableValue, IPickChildOperator {
            private Dictionary<string, VariableMemoryValue> _stringValues = new Dictionary<string, VariableMemoryValue>();
            private Dictionary<float, VariableMemoryValue> _floatValues = new Dictionary<float, VariableMemoryValue>();
            private Dictionary<int, VariableMemoryValue> _integerValues = new Dictionary<int, VariableMemoryValue>();
            
            /// <inheritdoc />
            public override SerializableValue Duplicate() {
                return new ObjectDelegate {
                    _stringValues = _stringValues.Duplicate(),
                    _floatValues = _floatValues.Duplicate(),
                    _integerValues = _integerValues.Duplicate()
                };
            }

            [NotNull]
            public VariableMemoryValue Add(SerializableValue name, SerializableValue value) {
                VariableMemoryValue result;
                switch (name) {
                    case FloatMemoryValue floatMemoryValue:
                        if (_floatValues.ContainsKey(floatMemoryValue.Value)) {
                            result = _floatValues[floatMemoryValue.Value];
                        } else {
                            result = new VariableMemoryValue {Value = value};
                            _floatValues.Add(floatMemoryValue.Value, result);
                        }
                        break;
                    case IntegerMemoryValue integerMemoryValue:
                        if (_integerValues.ContainsKey(integerMemoryValue.Value)) {
                            result = _integerValues[integerMemoryValue.Value];
                        } else {
                            result = new VariableMemoryValue {Value = value};
                            _integerValues.Add(integerMemoryValue.Value, result);
                        }
                        break;
                    case StringMemoryValue stringMemoryValue:
                        if (_stringValues.ContainsKey(stringMemoryValue.Value)) {
                            result = _stringValues[stringMemoryValue.Value];
                        } else {
                            result = new VariableMemoryValue {Value = value};
                            _stringValues.Add(stringMemoryValue.Value, result);
                        }
                        break;
                    case IFloatConverter floatConverter:
                        var floatValue = floatConverter.ConvertToFloat();
                        if (_floatValues.ContainsKey(floatValue)) {
                            result = _floatValues[floatValue];
                        } else {
                            result = new VariableMemoryValue {Value = value};
                            _floatValues.Add(floatValue, result);
                        }
                        break;
                    case IIntegerConverter integerConverter:
                        var integerValue = integerConverter.ConvertToInteger();
                        if (_integerValues.ContainsKey(integerValue)) {
                            result = _integerValues[integerValue];
                        } else {
                            result = new VariableMemoryValue {Value = value};
                            _integerValues.Add(integerValue, result);
                        }
                        break;
                    case IStringConverter stringConverter:
                        var stringValue = stringConverter.ConvertToString();
                        if (_stringValues.ContainsKey(stringValue)) {
                            result = _stringValues[stringValue];
                        } else {
                            result = new VariableMemoryValue {Value = value};
                            _stringValues.Add(stringValue, result);
                        }
                        break;
                    default:
                        var defaultStringValue = name.ToString();
                        if (_stringValues.ContainsKey(defaultStringValue)) {
                            result = _stringValues[defaultStringValue];
                        } else {
                            result = new VariableMemoryValue {Value = value};
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
                    case FloatMemoryValue floatMemoryValue:
                        result = _floatValues.ContainsKey(floatMemoryValue.Value) ? _floatValues[floatMemoryValue.Value] : null;
                        break;
                    case IntegerMemoryValue integerMemoryValue:
                        result = _integerValues.ContainsKey(integerMemoryValue.Value) ? _integerValues[integerMemoryValue.Value] : null;
                        break;
                    case StringMemoryValue stringMemoryValue:
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
                return result ?? Add(name, new NullMemoryValue());
            }
        }
    }
}