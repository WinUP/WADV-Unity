using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;

namespace WADV.VisualNovel.Plugin {
    /// <summary>
    /// 表示一个WADV插件或子插件
    /// </summary>
    public abstract class VisualNovelPlugin {
        /// <summary>
        /// 插件默认名称
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// 加载优先级
        /// </summary>
        internal int InitPriority { get; set; }

        /// <summary>
        /// 创建一个WADV插件
        /// </summary>
        /// <param name="name">插件名</param>
        /// <param name="priority">加载优先级</param>
        protected VisualNovelPlugin(string name, int priority = 0) {
            Name = name;
        }

        /// <summary>
        /// 执行当前插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public abstract Task<SerializableValue> Execute(ScriptRuntime context, IDictionary<SerializableValue, SerializableValue> parameters);

        /// <summary>
        /// 将此插件的数据转换为内存堆栈值
        /// </summary>
        /// <returns></returns>
        public virtual SerializableValue GetValue() {
            throw new NotSupportedException($"Unable to convert plugin {GetType().FullName} to SerializableValue: unsupported operation");
        }
    }
}