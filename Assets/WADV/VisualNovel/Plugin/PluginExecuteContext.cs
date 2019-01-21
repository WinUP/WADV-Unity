using System.Collections;
using System.Collections.Generic;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;

namespace WADV.VisualNovel.Plugin {
    /// <summary>
    /// 插件执行上下文
    /// </summary>
    public class PluginExecuteContext {
        /// <summary>
        /// 获取执行环境
        /// </summary>
        public ScriptRuntime Runtime { get; private set; }
        
        /// <summary>
        /// 获取完整参数列表
        /// </summary>
        public Dictionary<SerializableValue, SerializableValue> Parameters { get; }

        /// <summary>
        /// 获取当前激活的语言（等价于使用Runtime.ActiveLanguage）
        /// </summary>
        public string Language => Runtime?.ActiveLanguage;
        
        /// <summary>
        /// 获取名称可转换为字符串的参数列表
        /// </summary>
        public IEnumerable<KeyValuePair<IStringConverter, SerializableValue>> StringParameters => new StringParameterEnumerator(Parameters);

        private PluginExecuteContext(Dictionary<SerializableValue, SerializableValue> parameters) {
            Parameters = parameters;
        }

        /// <summary>
        /// 创建一个插件执行上下文
        /// </summary>
        /// <param name="runtime">执行环境</param>
        /// <returns></returns>
        public static PluginExecuteContext Create(ScriptRuntime runtime) {
            return new PluginExecuteContext(new Dictionary<SerializableValue, SerializableValue>()) {Runtime = runtime};
        }

        /// <summary>
        /// 插件执行上下文
        /// </summary>
        /// <param name="runtime">执行环境</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static PluginExecuteContext Create(ScriptRuntime runtime, Dictionary<SerializableValue, SerializableValue> parameters) {
            return new PluginExecuteContext(parameters) {Runtime = runtime};
        }

        private class StringParameterEnumerator : IEnumerator<KeyValuePair<IStringConverter, SerializableValue>>, IEnumerable<KeyValuePair<IStringConverter, SerializableValue>> {
            private readonly Dictionary<SerializableValue, SerializableValue> _parameters;
            private Dictionary<SerializableValue, SerializableValue>.Enumerator _enumerator;
            
            public StringParameterEnumerator(Dictionary<SerializableValue, SerializableValue> parameters) {
                _parameters = parameters;
                _enumerator = _parameters.GetEnumerator();
            }
            public bool MoveNext() {
                while (_enumerator.MoveNext()) {
                    if (!(_enumerator.Current.Key is IStringConverter stringConverter)) continue;
                    Current = new KeyValuePair<IStringConverter, SerializableValue>(stringConverter, _enumerator.Current.Value);
                    return true;
                }
                return false;
            }

            public void Reset() {
                _enumerator.Dispose();
                _enumerator = _parameters.GetEnumerator();
            }

            public KeyValuePair<IStringConverter, SerializableValue> Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() {
                _enumerator.Dispose();
            }

            public IEnumerator<KeyValuePair<IStringConverter, SerializableValue>> GetEnumerator() {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }
    }
}