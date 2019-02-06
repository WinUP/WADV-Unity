using System.Threading.Tasks;
using UnityEngine;
using WADV.Reflection;
using WADV.VisualNovel.Provider;

namespace WADV.Providers {
    /// <inheritdoc />
    /// <summary>
    /// 用于读取Unity中Resource文件夹内容的资源提供器
    /// </summary>
    [StaticRegistrationInfo("Resources")]
    public class UnityResourcesResourceProvider : IResourceProvider {
        /// <inheritdoc />
        public Task<object> Load(string id) {
            return Task.FromResult((object) Resources.Load(id));
        }
    }
}