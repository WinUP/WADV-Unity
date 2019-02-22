using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using WADV.Extensions;
using WADV.Thread;

namespace WADV.Plugins.Image.Effects {
    /// <inheritdoc cref="ShaderLoader" />
    /// <summary>
    /// 基于Shader的无限循环UI元素效果
    /// </summary>
    public abstract class StaticShaderGraphicEffect : StaticGraphicEffect {
        private readonly string _shaderName;
        protected Shader EffectShader;
        
        protected StaticShaderGraphicEffect(string shaderName) {
            _shaderName = shaderName;
        }

        public override async Task Initialize() {
            EffectShader = await ShaderLoader.Load(_shaderName);
        }
        
        /// <summary>
        /// 初始化材质
        /// </summary>
        /// <param name="nextTexture">播放完成后要显示的纹理</param>
        /// <returns></returns>
        protected abstract Material CreateMaterial([CanBeNull] Texture2D nextTexture);
        
        public async Task StartEffect(IEnumerable<Graphic> targets) {
            var material = CreateMaterial(Parameters);
            foreach (var graphic in targets.Where(e => !PlayingTargets.ContainsKey(e))) {
                graphic.material = material;
                PlayingTargets.Add(graphic, material);
            }
            await PlayStartEffect(material);
        }

        public void UpdateEffectFrame(Graphic[] targets) {
            foreach (var material in FindMaterials(targets)) {
                PlayFrameEffect(material);
            }
        }

        public async Task EndEffect(Graphic[] targets) {
            await Dispatcher.WaitAll(FindMaterials(targets).Select(PlayEndEffect).ToArray());
            foreach (var graphic in targets) {
                PlayingTargets.Remove(graphic);
            }
        }

        protected abstract Task PlayStartEffect(Material material);

        protected abstract void PlayFrameEffect(Material material);

        protected abstract Task PlayEndEffect(Material material);

        private static IEnumerable<Material> FindMaterials(IEnumerable<Graphic> targets) {
            var materials = new List<Material>();
            foreach (var graphic in targets) {
                var material = PlayingTargets[graphic];
                if (!materials.Contains(material)) {
                    materials.Add(material);
                }
            }
            return materials;
        }
    }
}