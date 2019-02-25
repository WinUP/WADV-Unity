using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Plugins.Image.Effects;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Effect")]
    [UsedImplicitly]
    public class EffectPlugin : IVisualNovelPlugin {
        private static readonly Dictionary<string, Type> Effects = new Dictionary<string, Type>();

        static EffectPlugin() {
            AssemblyRegister.Load(Assembly.GetExecutingAssembly());
        }
        
        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
        
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var parameters = new Dictionary<string, SerializableValue>();
            string effectName = null;
            var duration = 0.0F;
            var easingType = EasingType.Linear;
            foreach (var (key, value) in context.StringParameters) {
                var name = key.ConvertToString(context.Language);
                switch (name) {
                    case "Type":
                        effectName = StringValue.TryParse(value);
                        break;
                    case "Duration":
                        duration = FloatValue.TryParse(value);
                        break;
                    case "Easing": {
                        var easingName = StringValue.TryParse(value);
                        if (!Enum.TryParse<EasingType>(easingName, true, out var easing)) {
                            throw new NotSupportedException($"Unable to create effect: ease type {easingName} is not supported");
                        }
                        easingType = easing;
                        break;
                    }
                    default:
                        parameters.Add(name, value);
                        break;
                }
            }
            if (string.IsNullOrEmpty(effectName)) throw new NotSupportedException("Unable to create effect: missing effect type");
            if (duration.Equals(0.0F)) throw new NotSupportedException("Unable to create effect: missing duration or duration less than/equals to 0");
            var effect = Create(effectName, parameters, duration, easingType);
            if (effect == null) throw new KeyNotFoundException($"Unable to create effect: expected effect name {effectName} not existed");
            var result = new EffectValue(effect);
            return Task.FromResult<SerializableValue>(result);
        }

        [CanBeNull]
        public static GraphicEffect Create(string name, Dictionary<string, SerializableValue> parameters, float duration, EasingType easing) {
            return Effects.ContainsKey(name) ? GraphicEffect.CreateInstance<GraphicEffect>(Effects[name], parameters, duration, easing) : null;
        }

        [UsedImplicitly]
        private class AssemblyLoader : IAssemblyRegister {
            public void RegisterType(Type target, StaticRegistrationInfo info) {
                if (target.HasBase(typeof(GraphicEffect))) {
                    Effects.Add(AssemblyRegister.GetName(target), target);
                }
            }
        }
    }
}