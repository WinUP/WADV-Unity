using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.Thread;

namespace WADV.Plugins.Image.Effects {
    /// <inheritdoc cref="ILoopGraphicEffect" />
    /// <inheritdoc cref="ShaderGraphicEffect" />
    /// <summary>
    /// 基于Shader的无限循环UI元素效果
    /// </summary>
    public abstract class LoopShaderGraphicEffect : ShaderGraphicEffect, ILoopGraphicEffect {
        private static readonly Dictionary<Graphic, Material> PlayingTargets = new Dictionary<Graphic, Material>();
        /// <inheritdoc />
        /// <summary>
        /// 创建一个基于Shader的无限循环UI元素效果
        /// </summary>
        /// <param name="shaderName">Shader名称</param>
        public LoopShaderGraphicEffect(string shaderName) : base(shaderName) { }

        public async Task StartEffect(Graphic[] targets) {
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