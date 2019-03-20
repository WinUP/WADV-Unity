using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Extensions;
using WADV.Reflection;

namespace WADV.Resource.Providers {
    /// <inheritdoc cref="IResourceProvider" />
    /// <summary>
    /// 基于文件的AssetBundle资源提供器
    /// </summary>
    public class AssetBundleFileResourceProvider : IResourceProvider, IDynamicRegistrationTarget {
        public string RegistrationName { get; }
        
        private readonly string _fileName;
        [CanBeNull] private AssetBundle _assetBundle;

        /// <inheritdoc />
        /// <summary>
        /// 创建一个AssetBundle资源提供器
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="name">注册名</param>
        public AssetBundleFileResourceProvider(string fileName, string name) {
            _fileName = fileName;
            RegistrationName = name;
        }
        
        /// <inheritdoc />
        public async Task<object> Load(string id) {
            if (_assetBundle != null) return await _assetBundle.LoadAssetAsync(id);
            await ReadAssetBundle();
            if (_assetBundle != null) return await _assetBundle.LoadAssetAsync(id);
            Debug.LogError($"Unable to load {id}: cannot load asset bundle {_fileName} (resource provider will be unregistered)");
            ResourceManager.Unregister(this);
            return null;
        }

        private async Task ReadAssetBundle() {
            _assetBundle = await AssetBundle.LoadFromFileAsync(_fileName);
        }

        
    }
}