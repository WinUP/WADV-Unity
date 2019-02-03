using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.VisualNovel.Provider;

namespace WADV.Providers {
    /// <inheritdoc />
    /// <summary>
    /// 用于读取Unity中Persistent目录内容的资源提供器
    /// </summary>
    public class UnityPersistentResourceProvider : ResourceProvider {
        /// <inheritdoc />
        public UnityPersistentResourceProvider() : base("Persistent", 0) { }

        /// <inheritdoc />
        public override Task<object> Load(string id) {
            id = id.UnifySlash();
            var path = id.StartsWith("/") ? $"{Application.persistentDataPath}{id}" : $"{Application.persistentDataPath}/{id}";
            return File.Exists(path) ? Task.FromResult((object) new BinaryData(File.ReadAllBytes(path))) : null;
        }
    }
}