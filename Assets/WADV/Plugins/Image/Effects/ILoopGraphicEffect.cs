using System.Threading.Tasks;
using UnityEngine.UI;

namespace WADV.Plugins.Image.Effects {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个无限循环UI元素效果
    /// </summary>
    public interface ILoopGraphicEffect : IGraphicEffect {
        /// <summary>
        /// 启动效果
        /// </summary>
        /// <param name="targets">效果目标</param>
        /// <returns></returns>
        Task StartEffect(Graphic[] targets);
        
        /// <summary>
        /// 更新效果显示
        /// </summary>
        /// <param name="targets">效果目标</param>
        void UpdateEffectFrame(Graphic[] targets);

        /// <summary>
        /// 停止效果
        /// </summary>
        /// <param name="targets">效果目标</param>
        /// <returns></returns>
        Task EndEffect(Graphic[] targets);
    }
}