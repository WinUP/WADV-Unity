using System.Threading.Tasks;
using UnityEngine;
using WADV.VisualNovel.Provider;

namespace WADV.Providers {
    /// <inheritdoc />
    /// <summary>
    /// 用于读取Unity中Resource文件夹内容的资源提供器
    /// </summary>
    public class UnityResourcesResourceProvider : ResourceProvider {
        
        /// <inheritdoc />
        public UnityResourcesResourceProvider() : base("Resources", 0) { }
        
        /// <inheritdoc />
        public override Task<Object> Load(string id) {
            return Task.FromResult(Resources.Load(id));
        }
    }
}