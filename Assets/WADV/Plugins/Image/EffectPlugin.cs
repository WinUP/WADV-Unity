using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Plugins.Image.Effects;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Effect")]
    [UsedImplicitly]
    public class EffectPlugin : IVisualNovelPlugin {
        private static readonly Dictionary<string, Type> Effects = new Dictionary<string, Type>();

        static EffectPlugin() {
            AssemblyRegister.Load(Assembly.GetExecutingAssembly());
        }
        
        /// <inheritdoc />
        public bool OnRegister() => true;

        /// <inheritdoc />
        public bool OnUnregister(bool isReplace) => true;
        
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var parameters = new Dictionary<string, SerializableValue>();
            IGraphicEffect effect = null;
            string name = null;
            foreach (var (key, value) in context.StringParameters) {
                name = key.ConvertToString(context.Language);
                if (name == "Type") {
                    effect = Create(name);
                    if (effect == null) {
                        throw new KeyNotFoundException($"Unable to create effect: expected effect name {name} not existed");
                    }
                } else {
                    parameters.Add(name, value);
                }
            }
            if (effect == null) throw new NotSupportedException($"Unable to create effect: missing effect type");
            effect.CreateEffect(parameters);
            return Task.FromResult<SerializableValue>(new EffectValue(name, parameters) {Effect = effect});
        }

        [CanBeNull]
        public static IGraphicEffect Create(string name) {
            return Effects.ContainsKey(name) ? (IGraphicEffect) Activator.CreateInstance(Effects[name]) : null;
        }

        [UsedImplicitly]
        private class AssemblyLoader : IAssemblyRegister {
            public void RegisterType(Type target, StaticRegistrationInfo info) {
                if (target.GetInterfaces().Contains(typeof(IGraphicEffect))) {
                    Effects.Add(AssemblyRegister.GetName(target), target);
                }
            }
        }
    }
}