using System.Collections.Generic;
using Core.VisualNovel.StackItems;

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
        /// 
        /// </summary>
        PluginIdentifier Identifier { get; }

        /// <summary>
        /// 执行当前插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        IEnumerable<IStackItem> ExecuteAsync(ExecutionContext context, IReadOnlyDictionary<string, IStackItem> parameters);
        /// <summary>
        /// 从当前插件中获取指定名称的子插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <param name="childName">子插件名称</param>
        /// <returns></returns>
        IVisualNovelPlugin PickChild(ExecutionContext context, string childName);
    }
}