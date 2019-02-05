using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Reflection;

namespace WADV.VisualNovel.Plugin {
    /// <summary>
    /// 插件管理器
    /// </summary>
    public static class PluginManager {
        private static readonly Dictionary<string, IVisualNovelPlugin> Plugins = new Dictionary<string, IVisualNovelPlugin>();
        
        public class AutoRegister : IAssemblyRegister {
            public void RegisterType(Type target, UseStaticRegistration info, string name) {
                if (target.GetInterfaces().Contains(typeof(IVisualNovelPlugin))) {
                    Register((IVisualNovelPlugin) Activator.CreateInstance(target));
                }
            }
        }

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
                if (!plugin.OnUnregister(true)) throw new NotSupportedException($"Plugin {name} registration failed: conflict plugin denied to unregister");
                Plugins.Remove(name);
            }
            if (plugin.OnRegister()) {
                Plugins.Add(name, plugin);
            } else if (Application.isEditor) {
                Debug.LogWarning($"Plugin {name} registration failed: target plugin denied to register");
            }
        }

        /// <summary>
        /// 注销一个插件
        /// </summary>
        /// <param name="plugin">要注销的插件</param>
        public static void Unregister(IVisualNovelPlugin plugin) {
            var name = AssemblyRegister.GetName(plugin.GetType(), plugin);
            if (Plugins.ContainsKey(name)) {
                if (plugin.OnUnregister(false)) {
                    Plugins.Remove(name);
                } else if (Application.isEditor) {
                    Debug.LogWarning($"Plugin {name} unregisteration failed: target plugin denied to unregister");
                }
            }
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
    }
}