using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace WADV.Plugins.Image.Effects {
    public abstract class FrameShaderGraphicEffect : FrameGraphicEffect {
        protected abstract string ShaderName { get; }
        
        protected Shader EffectShader;

        public override async Task Initialize() {
            EffectShader = await ShaderLoader.Load(ShaderName);
        }
        
        /// <summary>
        /// 初始化材质
        /// </summary>
        /// <returns></returns>
        protected abstract Material CreateMaterial();
        
        public override async Task StartEffect(Graphic target) {
            var material = CreateMaterial();
            target.material = material;
            await PlayStartEffect(material);
        }

        public override async Task EndEffect(Graphic target) {
            await PlayEndEffect(target.material);
            target.material = null;
        }
        
        public override void UpdateEffect(Graphic target) {
            OnFrame(target.material);
        }

        protected abstract Task PlayStartEffect(Material material);

        protected abstract void OnFrame(Material material);

        protected abstract Task PlayEndEffect(Material material);
    }
}