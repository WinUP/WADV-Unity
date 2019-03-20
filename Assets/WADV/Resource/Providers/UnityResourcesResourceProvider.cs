using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Reflection;

namespace WADV.Resource.Providers {
    /// <inheritdoc />
    /// <summary>
    /// 用于读取Unity中Resource文件夹内容的资源提供器
    /// </summary>
    [StaticRegistrationInfo("Resources")]
    [UsedImplicitly]
    public class UnityResourcesResourceProvider : IResourceProvider {
        /// <inheritdoc />
        public Task<object> Load(string id) {
            return Task.FromResult((object) Resources.Load(id));
        }
    }
}