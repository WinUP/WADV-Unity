using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.VisualNovel.Attributes;
using Core.VisualNovel.Translation;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.VisualNovel.Plugin {
    /// <summary>
    /// 插件管理器
    /// </summary>
    public static class PluginManager {
        private static readonly Dictionary<string, IVisualNovelPlugin> Plugins = new Dictionary<string, IVisualNovelPlugin>();

        static PluginManager() {
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass && e.GetInterfaces().Contains(typeof(IVisualNovelPlugin)))) {
                try {
                    if (!(Activator.CreateInstance(item) is IVisualNovelPlugin plugin)) {
                        throw new MissingMemberException();
                    }
                    Register(plugin);
                } catch (MissingMemberException) {
                    Debug.Log($"Plugin {item.FullName} has no parameterless constructor, developer should register it to PluginManager manually to enable functions");
                }
            }
        }

        /// <summary>
        /// 根据名称寻找插件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [CanBeNull]
        public static IVisualNovelPlugin Find(string name, string language) {
            name = TranslationManager.GetPluginName(name, language);
            return Plugins.ContainsKey(name) ? Plugins[name] : null;
        }

        /// <summary>
        /// 注册一个插件
        /// <para>相同名称的插件会覆盖之前注册的插件并将在Unity控制台中显示警告，旧有插件的翻译也会被销毁</para>
        /// </summary>
        /// <param name="plugin">要注册的插件</param>
        public static void Register([NotNull] IVisualNovelPlugin plugin) {
            if (Plugins.ContainsKey(plugin.Name)) {
                Unregister(plugin);
            }
            Plugins.Add(plugin.Name, plugin);
            foreach (var translation in plugin.GetType().GetCustomAttributes<PluginTranslationAttribute>()) {
                TranslationManager.SetPluginName(plugin.Name, translation.Name, translation.Language);
            }
        }

        /// <summary>
        /// 注销一个插件
        /// </summary>
        /// <param name="plugin">要注销的插件</param>
        public static void Unregister(IVisualNovelPlugin plugin) {
            if (!Plugins.ContainsKey(plugin.Name)) {
                return;
            }
            Plugins.Remove(plugin.Name);
            TranslationManager.RemovePluginName(plugin.Name);
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