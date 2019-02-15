using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WADV.Plugins.Image.Effects {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个单程UI元素效果
    /// </summary>
    public interface ISingleGraphicEffect : IGraphicEffect {
        /// <summary>
        /// 播放效果
        /// </summary>
        /// <param name="targets">效果目标</param>
        /// <param name="totalTime">效果持续时间</param>
        /// <param name="easing">缓动函数</param>
        /// <param name="targetTexture">播放完成后要显示的纹理</param>
        /// <returns></returns>
        Task PlayEffect(IEnumerable<Graphic> targets, float totalTime, Func<float, float> easing, [CanBeNull] Texture2D targetTexture);
    }
}