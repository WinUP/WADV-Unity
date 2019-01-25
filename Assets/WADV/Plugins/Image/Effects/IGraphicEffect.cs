using UnityEngine.UI;

namespace WADV.Plugins.Image.Effects {
    /// <summary>
    /// 表示一个UI元素效果
    /// </summary>
    public interface IGraphicEffect {
        /// <summary>
        /// 初始化效果
        /// </summary>
        /// <param name="graphics">目标元素</param>
        void Initialize(Graphic[] graphics);

        /// <summary>
        /// 重置效果
        /// </summary>
        void Reset();
    }
}