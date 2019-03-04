using UnityEngine.UI;

namespace WADV.Plugins.Image.Effects {
    public abstract class FrameGraphicEffect : StaticGraphicEffect {
        /// <summary>
        /// 更新效果显示
        /// </summary>
        /// <param name="target">效果目标</param>
        public abstract void UpdateEffect(Graphic target);
    }
}