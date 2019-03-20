using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 插件管理器
    /// </summary>
    public static class PluginManager {
        private static readonly Dictionary<string, IVisualNovelPlugin> Plugins = new Dictionary<string, IVisualNovelPlugin>();

        static PluginManager() {
            AssemblyRegister.Load(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 根据名称寻找插件
        /// </summary>
        /// <param name="name">插件名</param>
        /// <returns></returns>
        [CanBeNull]
        public static IVisualNovelPlugin Find(string name) {
            return Plugins.ContainsKey(name) ? Plugins[name] : null;
        }

        /// <summary>
        /// 注册一个插件
        /// <para>相同名称的插件会覆盖之前注册的插件</para>
        /// </summary>
        /// <param name="plugin">要注册的插件</param>
        public static void Register([NotNull] IVisualNovelPlugin plugin) {
            var name = AssemblyRegister.GetName(plugin.GetType(), plugin);
            if (Plugins.ContainsKey(name)) {
                plugin.OnUnregister(true);
                Plugins.Remove(name);
            }
            plugin.OnRegister();
            Plugins.Add(name, plugin);
        }

        /// <summary>
        /// 注销一个插件
        /// </summary>
        /// <param name="plugin">要注销的插件</param>
        public static void Unregister(IVisualNovelPlugin plugin) {
            var name = AssemblyRegister.GetName(plugin.GetType(), plugin);
            if (!Plugins.ContainsKey(name)) return;
            plugin.OnUnregister(false);
            Plugins.Remove(name);
        }

        /// <summary>
        /// 检查指定插件是否已经注册
        /// </summary>
        /// <param name="plugin">目标插件</param>
        /// <returns></returns>
        public static bool Contains(IVisualNovelPlugin plugin) {
            return Plugins.ContainsValue(plugin);
        }

        /// <summary>
        /// 检查指定插件是否已经注册
        /// </summary>
        /// <param name="name">目标插件名</param>
        /// <returns></returns>
        public static bool Contains(string name) {
            return Plugins.ContainsKey(name);
        }
        
        [UsedImplicitly]
        private class AutoRegister : IAssemblyRegister {
            public void RegisterType(Type target, StaticRegistrationInfo info) {
                if (target.HasInterface(typeof(IVisualNovelPlugin))) {
                    Register((IVisualNovelPlugin) Activator.CreateInstance(target));
                }
            }
        }
    }
}