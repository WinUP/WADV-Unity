using System.Threading.Tasks;

namespace WADV.VisualNovel.Provider {
    /// <summary>
    /// 表示一个资源提供器
    /// </summary>
    public interface IResourceProvider {
        /// <summary>
        /// 读取资源
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns></returns>
        Task<object> Load(string id);
    }
}