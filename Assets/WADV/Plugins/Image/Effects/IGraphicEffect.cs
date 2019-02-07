using System.Collections.Generic;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    /// <summary>
    /// 表示一个UI元素效果
    /// </summary>
    public interface IGraphicEffect {
        /// <summary>
        /// 创建效果
        /// </summary>
        /// <param name="parameters">效果参数</param>
        void SetEffect(Dictionary<string, SerializableValue> parameters);
    }
}