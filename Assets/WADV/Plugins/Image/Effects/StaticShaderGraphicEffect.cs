using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.Resource;

namespace WADV.Plugins.Image.Effects {
    /// <inheritdoc />
    /// <summary>
    /// 基于Shader的无限循环UI元素效果
    /// </summary>
    public abstract class StaticShaderGraphicEffect : StaticGraphicEffect {
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

        protected abstract Task PlayStartEffect(Material material);

        protected abstract Task PlayEndEffect(Material material);
    }
}