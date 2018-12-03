using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.VisualNovel.Attributes;
using UnityEngine;

namespace Core.VisualNovel.Plugin {
    /// <summary>
    /// 插件管理器
    /// </summary>
    public static class PluginManager {
        private static readonly Dictionary<string, List<PluginDescription>> Descriptions = new Dictionary<string, List<PluginDescription>>();
        private static readonly Dictionary<PluginIdentifier, IVisualNovelPlugin> Plugins = new Dictionary<PluginIdentifier, IVisualNovelPlugin>();

        static PluginManager() {
            // 尝试自动加载所有具备无参构造函数的插件
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass)) {
                if (!item.GetInterfaces().Contains(typeof(IVisualNovelPlugin))) {
                    continue;
                }
                try {
                    if (!(Activator.CreateInstance(item) is IVisualNovelPlugin plugin)) {
                        throw new MissingMemberException();
                    }
                    Register(plugin);
                } catch (MissingMemberException) {
                    Debug.Log($"Plugin {item.FullName} has no parameterless constructor, compiler will not apply storage optimization and developer should register it to PluginManager manually to enable functions");
                }
            }
        }

        /// <summary>
        /// 根据插件名查找插件在指定语言下的描述
        /// </summary>
        /// <param name="language">描述语言</param>
        /// <param name="name">插件名</param>
        /// <returns></returns>
        public static PluginDescription? FindDescriptor(string language, string name) {
            if (!Descriptions.ContainsKey(language)) {
                return null;
            }
            foreach (var description in Descriptions[language]) {
                if (description.Name == name) {
                    return description;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 根据插件ID查找插件在指定语言下的描述
        /// </summary>
        /// <param name="language">描述语言</param>
        /// <param name="identifier">插件ID</param>
        /// <returns></returns>
        public static PluginDescription? FindDescriptor(string language, PluginIdentifier identifier) {
            if (!Descriptions.ContainsKey(language)) {
                return null;
            }
            foreach (var description in Descriptions[language]) {
                if (description.Identifier.IsSameId(identifier)) {
                    return description;
                }
            }
            return null;
        }

        /// <summary>
        /// 注册一个插件
        /// <para>相同ID的插件会覆盖之前注册的插件</para>
        /// </summary>
        /// <param name="plugin">要注册的插件</param>
        public static void Register(IVisualNovelPlugin plugin) {
            if (HasPluginRegistered(plugin)) {
                Unregister(plugin);
            }
            var translations = plugin.GetType().GetCustomAttributes<PluginTranslationAttribute>();
            foreach (var translation in translations) {
                AddExtraDescription(translation.Language, new PluginDescription(translation.Name, plugin.Identifier));
            }
            AddExtraDescription("default", new PluginDescription(plugin.Name, plugin.Identifier));
            Plugins.Add(plugin.Identifier, plugin);
        }

        /// <summary>
        /// 注销一个插件
        /// </summary>
        /// <param name="plugin">要注销的插件</param>
        public static void Unregister(IVisualNovelPlugin plugin) {
            foreach (var description in Descriptions) {
                description.Value.RemoveAll(e => e.Identifier.IsSameId(plugin.Identifier));
            }
            Plugins.Remove(plugin.Identifier);
        }

        /// <summary>
        /// 检查指定插件是否已经注册
        /// </summary>
        /// <param name="plugin">目标插件</param>
        /// <returns></returns>
        public static bool HasPluginRegistered(IVisualNovelPlugin plugin) {
            return HasPluginRegistered(plugin.Identifier);
        }

        /// <summary>
        /// 检查指定插件是否已经注册
        /// </summary>
        /// <param name="identifier">目标插件的ID</param>
        /// <returns></returns>
        public static bool HasPluginRegistered(PluginIdentifier identifier) {
            foreach (var plugin in Plugins) {
                if (plugin.Key.IsSameId(identifier)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加针对其他语言的插件描述
        /// </summary>
        /// <param name="language">描述语言</param>
        /// <param name="description">描述内容</param>
        public static void AddExtraDescription(string language, PluginDescription description) {
            List<PluginDescription> descriptions;
            if (!Descriptions.ContainsKey(language)) {
                descriptions = new List<PluginDescription>();
                Descriptions.Add(language, descriptions);
            } else {
                descriptions = Descriptions[language];
            }
            for (var i = -1; ++i < descriptions.Count;) {
                if (!descriptions[i].Identifier.IsSameId(description.Identifier)) continue;
                descriptions.RemoveAt(i);
                break;
            }
            descriptions.Add(description);
        }
    }
}