using System.Collections.Generic;
using UnityEngine.UI;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public abstract class FrameGraphicEffect : StaticGraphicEffect {
        /// <summary>
        /// 更新效果显示
        /// </summary>
        /// <param name="targets">效果目标</param>
        public abstract void UpdateEffect(IEnumerable<Graphic> targets);
    }
}