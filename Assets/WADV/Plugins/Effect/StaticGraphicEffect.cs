using System.Threading.Tasks;
using UnityEngine.UI;

namespace WADV.Plugins.Effect {
    public abstract class StaticGraphicEffect : GraphicEffect {
        /// <summary>
        /// 启动效果
        /// </summary>
        /// <param name="target">效果目标</param>
        /// <returns></returns>
        public abstract Task StartEffect(Graphic target);

        /// <summary>
        /// 停止效果
        /// </summary>
        /// <param name="target">效果目标</param>]
        /// <returns></returns>
        public abstract Task EndEffect(Graphic target);
    }
}