using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.Extensions;
using WADV.Thread;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    /// <inheritdoc cref="ISingleGraphicEffect" />
    /// <inheritdoc cref="ShaderGraphicEffect" />
    /// <summary>
    /// 基于Shader的单程UI元素效果
    /// </summary>
    public abstract class SingleShaderGraphicEffect : ShaderGraphicEffect, ISingleGraphicEffect {
        /// <inheritdoc />
        /// <summary>
        /// 创建一个基于Shader的单程UI元素效果
        /// </summary>
        /// <param name="shaderName">Shader名称</param>
        protected SingleShaderGraphicEffect(string shaderName) : base(shaderName) { }
        
        public async Task PlayEffect(IEnumerable<Graphic> targets, float totalTime, Func<float, float> easing) {
            var material = CreateMaterial(Parameters);
            foreach (var target in targets) {
                target.material = material;
            }
            var currentTime = 0.0F;
            while (currentTime < totalTime) {
                OnFrame(material, totalTime, Mathf.Clamp01(easing(currentTime / totalTime)));
                await Dispatcher.NextUpdate();
                currentTime += Time.deltaTime;
            }
            OnFrame(material, totalTime, 1.0F);
        }

        /// <summary>
        /// 逐帧更新材质参数
        /// </summary>
        /// <param name="material">目标材质</param>
        /// <param name="totalTime">效果预计播放时间</param>
        /// <param name="progress">效果播放进度</param>
        protected abstract void OnFrame(Material material, float totalTime, float progress);
    }
}