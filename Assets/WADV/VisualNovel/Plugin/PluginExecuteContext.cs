using System.Collections.Generic;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;

namespace WADV.VisualNovel.Plugin {
    /// <summary>
    /// 插件执行上下文
    /// </summary>
    public class PluginExecuteContext {
        /// <summary>
        /// 执行环境
        /// </summary>
        public ScriptRuntime Runtime { get; set; }
        
        /// <summary>
        /// 参数列表
        /// </summary>
        public Dictionary<SerializableValue, SerializableValue> Parameters { get; }

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
    }
}