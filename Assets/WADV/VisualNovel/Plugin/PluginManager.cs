using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace WADV.VisualNovel.Plugin {
    /// <summary>
    /// 插件管理器
    /// </summary>
    public static class PluginManager {
        private static readonly Dictionary<string, VisualNovelPlugin> Plugins = new Dictionary<string, VisualNovelPlugin>();

        static PluginManager() {
            var plugins = new List<VisualNovelPlugin>();
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(IsPlugin)) {
                try {
                    var plugin = (VisualNovelPlugin) Activator.CreateInstance(item);
                    plugins.Add(plugin);
                    if (Application.isEditor) {
                        Debug.Log($"Plugin manager: {item.FullName} register succeed");
                    }
                } catch (MissingMemberException) {
                    if (Application.isEditor) {
                        Debug.LogWarning($"Plugin manager: {item.FullName} register failed, manual registration required");
                    }
                } catch (Exception ex) {
                    Debug.LogError($"Plugin manager: {item.FullName} register failed with \"{ex}\"");
                }
            }
            foreach (var plugin in plugins.OrderByDescending(e => e.InitPriority)) {
                Register(plugin);
            }
        }

        /// <summary>
        /// 根据名称寻找插件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [CanBeNull]
        public static VisualNovelPlugin Find(string name) {
            return Plugins.ContainsKey(name) ? Plugins[name] : null;
        }

        /// <summary>
        /// 注册一个插件
        /// <para>相同名称的插件会覆盖之前注册的插件</para>
        /// </summary>
        /// <param name="plugin">要注册的插件</param>
        public static void Register([NotNull] VisualNovelPlugin plugin) {
            if (Plugins.ContainsKey(plugin.Name)) {
                if (!plugin.OnUnregister(true)) throw new NotSupportedException($"Plugin {plugin.Name} registration failed: conflict plugin denied to unregister");
                Plugins.Remove(plugin.Name);
            }
            if (plugin.OnRegister()) {
                Plugins.Add(plugin.Name, plugin);
            } else if (Application.isEditor) {
                Debug.LogWarning($"Plugin {plugin.Name} registration failed: target plugin denied to register");
            }
        }

        /// <summary>
        /// 注销一个插件
        /// </summary>
        /// <param name="plugin">要注销的插件</param>
        public static void Unregister(VisualNovelPlugin plugin) {
            if (Plugins.ContainsKey(plugin.Name)) {
                if (plugin.OnUnregister(false)) {
                    Plugins.Remove(plugin.Name);
                }else if (Application.isEditor) {
                    Debug.LogWarning($"Plugin {plugin.Name} unregisteration failed: target plugin denied to unregister");
                }
            }
        }

        /// <summary>
        /// 检查指定插件是否已经注册
        /// </summary>
        /// <param name="plugin">目标插件</param>
        /// <returns></returns>
        public static bool Contains(VisualNovelPlugin plugin) {
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

        private static bool IsPlugin(Type e) {
            if (!e.IsClass || e.IsAbstract) return false;
            var baseType = e;
            do {
                baseType = baseType.BaseType;
                if (baseType != null && baseType == typeof(VisualNovelPlugin)) return true;
            } while (baseType != null);
            return false;
        }
    }
}