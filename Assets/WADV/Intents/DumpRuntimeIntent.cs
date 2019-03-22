using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;

namespace WADV.Intents {
    [Serializable]
    public class DumpRuntimeIntent {
        public ScriptRuntime runtime;

        private Dictionary<string, int> _integerValue;
        private Dictionary<string, float> _floatValue;
        private Dictionary<string, string> _stringValue;
        private Dictionary<string, bool> _booleanValue;
        private Dictionary<string, SerializableValue> _serializableValue;

        public static DumpRuntimeIntent CreateEmpty() {
            return new DumpRuntimeIntent {
                _floatValue = new Dictionary<string, float>(),
                _stringValue = new Dictionary<string, string>(),
                _booleanValue = new Dictionary<string, bool>(),
                _integerValue = new Dictionary<string, int>(),
                _serializableValue = new Dictionary<string, SerializableValue>()
            };
        }

        public int GetInteger(string id) {
            return _integerValue.ContainsKey(id) ? _integerValue[id] : throw new KeyNotFoundException($"Unable to find key {id} in integer values");
        }

        public float GetFloat(string id) {
            return _floatValue.ContainsKey(id) ? _floatValue[id] : throw new KeyNotFoundException($"Unable to find key {id} in float values");
        }

        public string GetString(string id) {
            return _stringValue.ContainsKey(id) ? _stringValue[id] : throw new KeyNotFoundException($"Unable to find key {id} in string values");
        }
        
        public bool GetBoolean(string id) {
            return _booleanValue.ContainsKey(id) ? _booleanValue[id] : throw new KeyNotFoundException($"Unable to find key {id} in boolean values");
        }

        [CanBeNull]
        public T GetValue<T>(string id) where T : SerializableValue {
            return _serializableValue.ContainsKey(id) ? _serializableValue[id] as T : throw new KeyNotFoundException($"Unable to find key {id} in serializable values");
        }

        public void AddValue(string id, int value) {
            if (_integerValue.ContainsKey(id)) {
                _integerValue[id] = value;
            } else {
                _integerValue.Add(id, value);
            }
        }
        
        public void AddValue(string id, float value) {
            if (_floatValue.ContainsKey(id)) {
                _floatValue[id] = value;
            } else {
                _floatValue.Add(id, value);
            }
        }
        
        public void AddValue(string id, string value) {
            if (_stringValue.ContainsKey(id)) {
                _stringValue[id] = value;
            } else {
                _stringValue.Add(id, value);
            }
        }
        
        public void AddValue(string id, bool value) {
            if (_booleanValue.ContainsKey(id)) {
                _booleanValue[id] = value;
            } else {
                _booleanValue.Add(id, value);
            }
        }

        public void AddValue(string id, SerializableValue value) {
            if (_serializableValue.ContainsKey(id)) {
                _serializableValue[id] = value;
            } else {
                _serializableValue.Add(id, value);
            }
        }
    }
}