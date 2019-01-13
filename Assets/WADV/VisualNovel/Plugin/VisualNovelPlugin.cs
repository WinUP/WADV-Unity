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
        internal int InitPriority { get; }

        /// <summary>
        /// 创建一个WADV插件
        /// </summary>
        /// <param name="name">插件名</param>
        /// <param name="priority">加载优先级</param>
        protected VisualNovelPlugin(string name, int priority = 0) {
            Name = name;
            InitPriority = priority;
        }

        /// <summary>
        /// 执行当前插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <returns></returns>
        public abstract Task<SerializableValue> Execute(PluginExecuteContext context);

        /// <summary>
        /// 检查是否允许插件被注册
        /// </summary>
        public virtual bool OnRegister() {
            return true;
        }

        /// <summary>
        /// 检查是否允许插件被注销
        /// </summary>
        /// <param name="isReplace">是否由于同名插件注册使得此插件被注销</param>
        public virtual bool OnUnregister(bool isReplace) {
            return true;
        }
    }
}