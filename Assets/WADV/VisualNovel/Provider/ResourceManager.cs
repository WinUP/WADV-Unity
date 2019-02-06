using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;

namespace WADV.VisualNovel.Provider {
    /// <summary>
    /// 资源提供器管理器
    /// </summary>
    public static class ResourceManager {
        private static readonly Dictionary<string, IResourceProvider> Providers = new Dictionary<string, IResourceProvider>();
        
        static ResourceManager() {
            AssemblyRegister.Load(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 获取资源提供器迭代器
        /// </summary>
        public static IEnumerator<KeyValuePair<string, IResourceProvider>> GetEnumerator => Providers.GetEnumerator();

        /// <summary>
        /// 根据名称寻找资源提供器
        /// </summary>
        /// <param name="name">提供器名</param>
        /// <returns></returns>
        [CanBeNull]
        public static IResourceProvider Find(string name) {
            return Providers.ContainsKey(name) ? Providers[name] : null;
        }
        
        /// <summary>
        /// 注册一个资源提供器
        /// <para>相同名称的提供器会覆盖之前注册的提供器</para>
        /// </summary>
        /// <param name="plugin">要注册的提供器</param>
        public static void Register([NotNull] IResourceProvider plugin) {
            var name = AssemblyRegister.GetName(plugin.GetType(), plugin);
            Providers.TryRemove(name);
            Providers.Add(name, plugin);
        }
        
        /// <summary>
        /// 注销一个资源提供器
        /// </summary>
        /// <param name="plugin">要注销的提供器</param>
        public static void Unregister(IResourceProvider plugin) {
            var name = AssemblyRegister.GetName(plugin.GetType(), plugin);
            Providers.TryRemove(name);
        }
        
        /// <summary>
        /// 检查指定资源提供器是否已经注册
        /// </summary>
        /// <param name="plugin">目标提供器</param>
        /// <returns></returns>
        public static bool Contains(IResourceProvider plugin) {
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
        /// <param name="address">资源地址</param>
        /// <returns></returns>
        public static async Task<object> Load(string address) {
            var splitter = address.IndexOf("://", StringComparison.Ordinal);
            if (splitter < 1) throw new FormatException($"Unable to load resource: address {address} must has format {{provider}}://{{id}}");
            var providerName = address.Substring(0, splitter);
            var provider = Find(providerName);
            if (provider == null) throw new KeyNotFoundException($"Unable to load resource: expected provider {providerName} not existed");
            return await provider.Load(address.Substring(splitter + 3));
        }

        /// <summary>
        /// 读取资源并按如下优先级尝试进行格式转换
        /// <list type="bullet">
        ///   <item><description>如果要求返回字符串，且提供器返回BinaryData，则取该对象的UTF-8文本表示</description></item>
        ///   <item><description>如果要求返回字符串，且提供器返回IStringConverter，则取该对象在默认语言下的值</description></item>
        ///   <item><description>如果要求返回字符串，且提供器没有返回上述对象，则强制转换为字符串</description></item>
        ///   <item><description>如果提供器返回的对象可以强制转换为目标对象则进行强制转换</description></item>
        ///   <item><description>如果读取到BinaryData或byte数组则尝试反序列化二进制内容为指定类型</description></item>
        /// </list>
        /// </summary>
        /// <param name="address">资源地址</param>
        /// <returns></returns>
        public static async Task<T> Load<T>(string address) where T : class {
            var result = await Load(address);
            if (result == null) return null;
            if (typeof(T) == typeof(string))
                switch (result) {
                    case BinaryData binaryResult:
                        return binaryResult.Text as T;
                    case IStringConverter stringResult:
                        return stringResult.ConvertToString() as T;
                    default:
                        return result.ToString() as T;
                }
            if (result is T tryCaseResult) return tryCaseResult;
            byte[] bytes = null;
            switch (result) {
                case BinaryData binaryResult:
                    bytes = binaryResult.Data;
                    break;
                case byte[] byteResult:
                    bytes = byteResult;
                    break;
            }
            if (bytes != null) {
                try {
                    var deserializer = new BinaryFormatter();
                    var stream = new MemoryStream(bytes);
                    var item = deserializer.Deserialize(stream);
                    stream.Close();
                    return item.GetType() == typeof(T) ? (T) item : null;
                } catch {
                    return null;
                }
            }
            return null;
        }
        
        private class AutoRegister : IAssemblyRegister {
            public void RegisterType(Type target, StaticRegistrationInfo info) {
                if (target.HasInterface(typeof(IResourceProvider))) {
                    Register((IResourceProvider) Activator.CreateInstance(target));
                }
            }
        }
    }
}