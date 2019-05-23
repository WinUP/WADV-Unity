using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WADV.Plugins.Effect {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个单程UI特效
    /// </summary>
    public abstract class SingleGraphicEffect : GraphicEffect {
        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="targets">播放目标</param>
        /// <param name="nextTexture">播放结束后预计使用的材质</param>
        /// <returns></returns>
        public abstract Task PlayEffect(IEnumerable<Graphic> targets, [CanBeNull] Texture2D nextTexture);
    }
}