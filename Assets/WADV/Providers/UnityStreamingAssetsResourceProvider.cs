using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Provider;

namespace WADV.Providers {
    [UseStaticRegistration("StreamingAssets")]
    public class UnityStreamingAssetsResourceProvider : IResourceProvider {
        public Task<object> Load(string id) {
            id = id.UnifySlash();
            var path = id.StartsWith("/") ? $"{Application.streamingAssetsPath}{id}" : $"{Application.streamingAssetsPath}/{id}";
            return File.Exists(path) ? Task.FromResult((object) new BinaryData(File.ReadAllBytes(path))) : null;
        }
    }
}