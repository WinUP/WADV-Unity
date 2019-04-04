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
    /// 基于Shader的单程UI特效
    /// </summary>
    public abstract class SingleShaderGraphicEffect : SingleGraphicEffect {
        protected abstract string ShaderName { get; }
        
        protected Shader EffectShader;
        public override async Task Initialize() {
            EffectShader = await ShaderLoader.Load(ShaderName);
        }

        public override async Task PlayEffect(IEnumerable<Graphic> targets, Texture2D nextTexture) {
            var material = CreateMaterial(nextTexture);
            targets = targets.ToArray();
            foreach (var target in targets) {
                target.material = material;
            }
            var currentTime = 0.0F;
            while (currentTime < Duration) {
                OnFrame(material, GetProgress(currentTime));
                await Dispatcher.NextUpdate();
                currentTime += Time.deltaTime;
            }
            OnFrame(material, 1.0F);
            foreach (var target in targets) {
                target.material = null;
            }
        }
        
        /// <summary>
        /// 初始化材质
        /// </summary>
        /// <param name="nextTexture">播放完成后要显示的纹理</param>
        /// <returns></returns>
        protected abstract Material CreateMaterial([CanBeNull] Texture2D nextTexture);

        /// <summary>
        /// 逐帧更新材质参数
        /// </summary>
        /// <param name="material">目标材质</param>
        /// <param name="progress">特效播放进度</param>
        protected abstract void OnFrame(Material material, float progress);
    }
}