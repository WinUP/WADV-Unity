using System.Threading.Tasks;

namespace WADV.VisualNovel.Provider {
    /// <summary>
    /// 表示一个资源提供器
    /// </summary>
    public abstract class ResourceProvider {
        /// <summary>
        /// 资源提供器默认名称
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// 加载优先级
        /// </summary>
        internal int InitPriority { get; }
        
        /// <summary>
        /// 创建一个资源提供器
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="priority">加载优先级（越大越优先）</param>
        protected ResourceProvider(string name, int priority = 0) {
            Name = name;
            InitPriority = priority;
        }

        /// <summary>
        /// 读取资源
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns></returns>
        public abstract Task<object> Load(string id);
    }
}