using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using WADV.Extensions;

namespace WADV.Reflection {
    /// <summary>
    /// 程序集自动注册器
    /// </summary>
    public static class AssemblyRegister {
        private static readonly List<string> LoadedAssemblies = new List<string>();
        private static readonly List<IAssemblyRegister> Registers = new List<IAssemblyRegister>();

        static AssemblyRegister() {
            var registerType = typeof(IAssemblyRegister);
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass && !e.IsAbstract && e.HasInterface(registerType))) {
                Registers.Add((IAssemblyRegister) Activator.CreateInstance(item));
            }
        }
        
        /// <summary>
        /// 扫描程序集并注册组件
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        public static void Load(Assembly assembly) {
            if (LoadedAssemblies.Contains(assembly.FullName)) return;
            LoadedAssemblies.Add(assembly.FullName);
            var targets = new List<(Type Item, StaticRegistrationInfoAttribute Information)>();
            foreach (var item in assembly.GetTypes().Where(e => e.IsClass && !e.IsAbstract && e.GetCustomAttribute<StaticRegistrationInfoAttribute>() != null)) {
                var info = item.GetCustomAttribute<StaticRegistrationInfoAttribute>();
                if (targets.Any()) {
                    var index = targets.FindIndex(e => e.Information.Priority > info.Priority);
                    if (index < 0) {
                        targets.Add((item, info));
                    } else {
                        targets.Insert(index, (item, info));
                    }
                } else {
                    targets.Add((item, info));
                }
            }
            foreach (var (item, information) in targets) {
                foreach (var register in Registers) {
                    register.RegisterType(item, information);
                }
            }
        }

        /// <summary>
        /// 获取自动注册对象的注册名称
        /// </summary>
        /// <param name="target">对象类型</param>
        /// <param name="instance">对象实例</param>
        /// <returns></returns>
        public static StaticRegistrationInfoAttribute[] GetInfo(Type target, [CanBeNull] object instance = null) {
            if (instance != null && instance is IDynamicRegistrationTarget dynamicRegisterTarget) return dynamicRegisterTarget.RegistrationInfo;
            var descriptions = target.GetCustomAttributes<StaticRegistrationInfoAttribute>().ToArray();
            return descriptions.Length == 0
                ? new[] {new StaticRegistrationInfoAttribute(target.Name)}
                : descriptions.ToArray();
        }
    }
}