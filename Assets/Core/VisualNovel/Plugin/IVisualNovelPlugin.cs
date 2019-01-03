using System.Collections.Generic;
using System.Threading.Tasks;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Runtime;

namespace Core.VisualNovel.Plugin {
    /// <summary>
    /// 表示一个WADV插件或子插件
    /// </summary>
    public interface IVisualNovelPlugin {
        /// <summary>
        /// 插件默认名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行当前插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        Task<ISerializableValue> Execute(ScriptRuntime context, IDictionary<ISerializableValue, ISerializableValue> parameters);
        
        /// <summary>
        /// 从当前插件中获取指定名称的子插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <param name="childName">子插件名称</param>
        /// <returns></returns>
        IVisualNovelPlugin PickChild(ScriptRuntime context, string childName);

        /// <summary>
        /// 将此插件的数据转换为内存堆栈值
        /// </summary>
        /// <returns></returns>
        ISerializableValue ToValue();
    }
}