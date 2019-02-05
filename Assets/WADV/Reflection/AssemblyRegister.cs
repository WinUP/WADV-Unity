using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
using UnityEngine;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Provider;

namespace WADV.Reflection {
    /// <summary>
    /// 自动注册器
    /// </summary>
    public static class AssemblyRegister {
        private static readonly List<string> LoadedAssemblies = new List<string>();
        private static readonly List<IAssemblyRegister> Registers = new List<IAssemblyRegister>();

        static AssemblyRegister() {
            var registerType = typeof(IAssemblyRegister);
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass && !e.IsAbstract && e.GetInterfaces().Contains(registerType))) {
                Registers.Add(Activator.CreateInstance(item) as IAssemblyRegister);
            }
        }
        
        /// <summary>
        /// 扫描程序集并注册组件
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        public static void Load(Assembly assembly) {
            if (LoadedAssemblies.Contains(assembly.FullName)) return;
            LoadedAssemblies.Add(assembly.FullName);
            foreach (var item in assembly.GetTypes().Where(e => e.IsClass && !e.IsAbstract && e.GetCustomAttribute<UseStaticRegistration>() != null)) {
                foreach (var register in Registers) {
                    register.RegisterType(item, item.GetCustomAttribute<UseStaticRegistration>(), GetName(item));
                }
            }
        }

        /// <summary>
        /// 获取自动注册对象的注册名称
        /// </summary>
        /// <param name="target">对象类型</param>
        /// <param name="instance">对象实例</param>
        /// <returns></returns>
        public static string GetName(Type target, [CanBeNull] object instance = null) {
            if (instance != null && instance is IDynamicRegistrationTarget dynamicRegisterTarget) return dynamicRegisterTarget.RegistrationName;
            var description = target.GetCustomAttribute<UseStaticRegistration>();
            return description == null ? target.Name : description.Name;
        }

        public static bool HasBase(Type target, Type targetBase) {
            var baseType = target;
            do {
                baseType = baseType.BaseType;
                if (baseType != null && baseType == targetBase) return true;
            } while (baseType != null);
            return false;
        }
    }
}