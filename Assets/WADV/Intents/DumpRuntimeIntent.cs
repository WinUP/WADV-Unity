using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Thread;
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
        private Dictionary<string, object> _objectValue;

        public static DumpRuntimeIntent CreateEmpty() {
            return new DumpRuntimeIntent {
                _floatValue = new Dictionary<string, float>(),
                _stringValue = new Dictionary<string, string>(),
                _booleanValue = new Dictionary<string, bool>(),
                _integerValue = new Dictionary<string, int>(),
                _objectValue = new Dictionary<string, object>()
            };
        }

        public int GetInteger(string id) {
            return _integerValue.ContainsKey(id) ? _integerValue[id] : throw new KeyNotFoundException($"Unable to get dump data: missing key {id} in integer values");
        }

        public float GetFloat(string id) {
            return _floatValue.ContainsKey(id) ? _floatValue[id] : throw new KeyNotFoundException($"Unable to get dump data: missing key {id} in float values");
        }

        public string GetString(string id) {
            return _stringValue.ContainsKey(id) ? _stringValue[id] : throw new KeyNotFoundException($"Unable to get dump data: missing key {id} in string values");
        }
        
        public bool GetBoolean(string id) {
            return _booleanValue.ContainsKey(id) ? _booleanValue[id] : throw new KeyNotFoundException($"Unable to get dump data: missing key {id} in boolean values");
        }

        [CanBeNull]
        public T GetValue<T>(string id) where T : class {
            return _objectValue.ContainsKey(id) ? _objectValue[id] as T : throw new KeyNotFoundException($"Unable to get dump data: missing key {id} in object values");
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

        public void AddValue(string id, object value) {
            if (!value.GetType().IsSerializable)
                throw new NotSupportedException($"Unable to set dump data {id}: target {value.GetType().FullName} is not serializable");
            if (_objectValue.ContainsKey(id)) {
                _objectValue[id] = value;
            } else {
                _objectValue.Add(id, value);
            }
        }

        public class TaskLists {
            private readonly Dictionary<SerializableValue, Task> _tasks = new Dictionary<SerializableValue, Task>();

            public void OnDump(SerializableValue value) {
                _tasks.Add(value, value.OnDump(this));
            }

            public void OnRead(SerializableValue value) {
                _tasks.Add(value, value.OnRead(this));
            }

            public Task WaitAll() {
                return Dispatcher.WaitAll(_tasks.Values);
            }
        }
    }
}