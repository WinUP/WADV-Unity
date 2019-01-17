using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Attributes;
using WADV.Extensions;
using WADV.VisualNovel.Provider;

namespace WADV.Providers {
    /// <inheritdoc />
    /// <summary>
    /// 基于文件的AssetBundle资源提供器
    /// </summary>
    [SkipAutoRegistration]
    public class AssetBundleFileResourceProvider : ResourceProvider {
        private readonly string _fileName;
        [CanBeNull] private AssetBundle _assetBundle;

        /// <inheritdoc />
        /// <summary>
        /// 创建一个AssetBundle资源提供器
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="name">名称</param>
        /// <param name="priority">加载优先级（越大越优先）</param>
        public AssetBundleFileResourceProvider(string fileName, string name, int priority = 0) : base(name, priority) {
            _fileName = fileName;
        }
        
        /// <inheritdoc />
        public override async Task<Object> Load(string id) {
            if (_assetBundle == null) {
                await ReadAssetBundle();
            }
            if (_assetBundle == null) {
                Debug.LogError($"Unable to load {id}: cannot load asset bundle {_fileName} (resource provider will be unregistered)");
                ResourceProviderManager.Unregister(this);
                return null;
            }
            return await _assetBundle.LoadAssetAsync(id);
        }

        private async Task ReadAssetBundle() {
            _assetBundle = await AssetBundle.LoadFromFileAsync(_fileName);
        }
    }
}