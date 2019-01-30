using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.VisualNovel.Provider;

namespace WADV.Providers {
    public class UnityStreamingAssetsResourceProvider : ResourceProvider {
        public UnityStreamingAssetsResourceProvider() : base("StreamingAssets", 0) { }

        public override Task<object> Load(string id) {
            id = id.UnifySlash();
            var path = id.StartsWith("/") ? $"{Application.streamingAssetsPath}{id}" : $"{Application.streamingAssetsPath}/{id}";
            return File.Exists(path) ? Task.FromResult((object) new BinaryData(File.ReadAllBytes(path))) : null;
        }
    }
}