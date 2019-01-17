using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WADV.Attributes;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Provider;

namespace WADV {
    /// <summary>
    /// 自动注册器
    /// </summary>
    public static class AutoRegister {
        private static readonly List<string> LoadedAssemblies = new List<string>();
        
        /// <summary>
        /// 扫描程序集并注册组件
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        public static void Load(Assembly assembly) {
            if (LoadedAssemblies.Contains(assembly.FullName)) return;
            LoadedAssemblies.Add(assembly.FullName);
            foreach (var item in assembly.GetTypes().Where(e => e.IsClass && !e.IsAbstract && e.GetCustomAttribute<SkipAutoRegistrationAttribute>() == null)) {
                try {
                    var (isPlugin, isProvider) = Verify(item);
                    if (isPlugin) {
                        PluginManager.Register((VisualNovelPlugin) Activator.CreateInstance(item));
                        if (Application.isEditor) {
                            Debug.Log($"Auto register: add {item.FullName} as visual novel plugin");
                        }
                    }
                    if (isProvider) {
                        ResourceProviderManager.Register((ResourceProvider) Activator.CreateInstance(item));
                        if (Application.isEditor) {
                            Debug.Log($"Auto register: add {item.FullName} as resource provider");
                        }
                    }
                } catch (MissingMemberException) {
                    if (Application.isEditor) {
                        Debug.LogWarning($"Auto register: {item.FullName} register failed, manual registration required");
                    }
                } catch (Exception ex) {
                    Debug.LogError($"Auto register: {item.FullName} register failed with \"{ex}\"");
                }
            }
        }

        private static (bool IsPlugin, bool IsProvider) Verify(Type e) {
            var baseType = e;
            var isPlugin = false;
            var isProvider = false;
            do {
                baseType = baseType.BaseType;
                if (baseType != null && baseType == typeof(ResourceProvider)) {
                    isProvider = true;
                }
                if (baseType != null && baseType == typeof(VisualNovelPlugin)) {
                    isPlugin = true;
                }
            } while (baseType != null);
            return (isPlugin, isProvider);
        }
    }
}