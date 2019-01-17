using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WADV.VisualNovel.Provider {
    /// <summary>
    /// 资源提供器管理器
    /// </summary>
    public static class ResourceProviderManager {
        private static readonly Dictionary<string, Provider.ResourceProvider> Providers = new Dictionary<string, Provider.ResourceProvider>();
        
        static ResourceProviderManager() {
            AutoRegister.Load(Assembly.GetExecutingAssembly());
        }
        
        /// <summary>
        /// 根据名称寻找资源提供器
        /// </summary>
        /// <param name="name">提供器名</param>
        /// <returns></returns>
        [CanBeNull]
        public static Provider.ResourceProvider Find(string name) {
            return Providers.ContainsKey(name) ? Providers[name] : null;
        }
        
        /// <summary>
        /// 注册一个资源提供器
        /// <para>相同名称的提供器会覆盖之前注册的提供器</para>
        /// </summary>
        /// <param name="plugin">要注册的提供器</param>
        public static void Register([NotNull] Provider.ResourceProvider plugin) {
            if (Providers.ContainsKey(plugin.Name)) {
                Providers.Remove(plugin.Name);
            }
            Providers.Add(plugin.Name, plugin);
        }
        
        /// <summary>
        /// 注销一个资源提供器
        /// </summary>
        /// <param name="plugin">要注销的提供器</param>
        public static void Unregister(Provider.ResourceProvider plugin) {
            if (Providers.ContainsKey(plugin.Name)) {
                Providers.Remove(plugin.Name);
            }
        }
        
        /// <summary>
        /// 检查指定资源提供器是否已经注册
        /// </summary>
        /// <param name="plugin">目标提供器</param>
        /// <returns></returns>
        public static bool Contains(Provider.ResourceProvider plugin) {
            return Providers.ContainsValue(plugin);
        }
        
        /// <summary>
        /// 检查指定资源提供器是否已经注册
        /// </summary>
        /// <param name="name">目标提供器名</param>
        /// <returns></returns>
        public static bool Contains(string name) {
            return Providers.ContainsKey(name);
        }

        /// <summary>
        /// 读取资源
        /// </summary>
        /// <param name="address">资源地址（必须符合格式[provider]:[id]）</param>
        /// <returns></returns>
        public static async Task<object> Load(string address) {
            var splitter = address.IndexOf(':');
            if (splitter < 1) throw new FormatException($"Unable to load resource: address {address} must has format [provider]:[id]");
            var providerName = address.Substring(0, splitter);
            var provider = Find(providerName);
            if (provider == null) throw new KeyNotFoundException($"Unable to load resource: expected provider {providerName} not existed");
            return await provider.Load(address.Substring(splitter + 1));
        }

        /// <summary>
        /// 读取资源
        /// </summary>
        /// <param name="address">资源地址（必须符合格式[provider]:[id]）</param>
        /// <returns></returns>
        public static async Task<T> Load<T>(string address) where T : class {
            var result = await Load(address);
            return result == null || result.GetType() != typeof(T) ? null : (T) result;
        }
    }
}