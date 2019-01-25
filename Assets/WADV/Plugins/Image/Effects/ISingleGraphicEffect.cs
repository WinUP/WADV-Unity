using System;
using System.Threading.Tasks;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个单程UI元素效果
    /// </summary>
    public interface ISingleGraphicEffect : IGraphicEffect {
        /// <summary>
        /// 应用效果
        /// </summary>
        /// <param name="totalTime">效果持续时间</param>
        /// <param name="easing">缓动函数</param>
        /// <param name="parameters">效果参数</param>
        /// <returns></returns>
        Task Apply(float totalTime, Func<float, float> easing, SerializableValue[] parameters);
    }
}