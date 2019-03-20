using System.Threading.Tasks;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个WADV插件
    /// </summary>
    public interface IVisualNovelPlugin {
        /// <summary>
        /// 执行当前插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <returns></returns>
        Task<SerializableValue> Execute(PluginExecuteContext context);

        /// <summary>
        /// 检查是否允许插件被注册
        /// </summary>
        void OnRegister();

        /// <summary>
        /// 检查是否允许插件被注销
        /// </summary>
        /// <param name="isReplace">是否由于同名插件注册使得此插件被注销</param>
        void OnUnregister(bool isReplace);
    }
}