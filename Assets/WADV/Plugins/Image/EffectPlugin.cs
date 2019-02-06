using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADV.Plugins.Image.Effects;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("Effect")]
    public class EffectPlugin : IVisualNovelPlugin, IAssemblyRegister {
        private readonly Dictionary<string, Type> _effects = new Dictionary<string, Type>();
        /// <inheritdoc />
        public bool OnRegister() => true;

        /// <inheritdoc />
        public bool OnUnregister(bool isReplace) => true;
        
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            throw new System.NotImplementedException();
        }

        public void RegisterType(Type target, StaticRegistrationInfo info) {
            if (target.GetInterfaces().Contains(typeof(IGraphicEffect))) {
                _effects.Add(AssemblyRegister.GetName(target), target);
            }
        }
    }
}