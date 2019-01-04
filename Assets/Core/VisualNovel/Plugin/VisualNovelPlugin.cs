using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Runtime;

namespace Core.VisualNovel.Plugin {
    /// <summary>
    /// 表示一个WADV插件或子插件
    /// </summary>
    public abstract class VisualNovelPlugin {
        /// <summary>
        /// 插件默认名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 创建一个WADV插件
        /// </summary>
        /// <param name="name">插件名</param>
        protected VisualNovelPlugin(string name) {
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
        /// 从当前插件中获取指定名称的子插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <param name="childName">子插件名称</param>
        /// <returns></returns>
        public virtual VisualNovelPlugin PickChild(ScriptRuntime context, string childName) {
            throw new NotSupportedException($"Unable to pick child {childName} from plugin {GetType().FullName}: unsupported operation");
        }

        /// <summary>
        /// 将此插件的数据转换为内存堆栈值
        /// </summary>
        /// <returns></returns>
        public virtual SerializableValue GetValue() {
            throw new NotSupportedException($"Unable to convert plugin {GetType().FullName} to SerializableValue: unsupported operation");
        }
    }
}