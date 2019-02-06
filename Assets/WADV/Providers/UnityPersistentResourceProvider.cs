using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Provider;

namespace WADV.Providers {
    /// <inheritdoc />
    /// <summary>
    /// 用于读取Unity中Persistent目录内容的资源提供器
    /// </summary>
    [StaticRegistrationInfo("Persistent")]
    public class UnityPersistentResourceProvider : IResourceProvider {
        /// <inheritdoc />
        public Task<object> Load(string id) {
            id = id.UnifySlash();
            var path = id.StartsWith("/") ? $"{Application.persistentDataPath}{id}" : $"{Application.persistentDataPath}/{id}";
            return File.Exists(path) ? Task.FromResult((object) new BinaryData(File.ReadAllBytes(path))) : null;
        }
    }
}