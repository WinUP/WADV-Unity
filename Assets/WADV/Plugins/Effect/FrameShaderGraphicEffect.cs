using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.Resource;

namespace WADV.Plugins.Effect {
    public abstract class FrameShaderGraphicEffect : FrameGraphicEffect {
        protected abstract string ShaderId { get; }
        
        protected Shader EffectShader;

        public override async Task Initialize() {
            EffectShader = await ResourceManager.Load<Shader>($"Shader://{ShaderId}");
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