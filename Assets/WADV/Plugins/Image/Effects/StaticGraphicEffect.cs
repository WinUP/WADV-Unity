using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public abstract class StaticGraphicEffect : GraphicEffect {
        /// <summary>
        /// 启动效果
        /// </summary>
        /// <param name="targets">效果目标</param>
        /// <returns></returns>
        public abstract Task StartEffect(IEnumerable<Graphic> targets);

        /// <summary>
        /// 停止效果
        /// </summary>
        /// <param name="targets">效果目标</param>
        /// <param name="nextTexture">播放结束后预计使用的材质</param>
        /// <returns></returns>
        public abstract Task EndEffect(IEnumerable<Graphic> targets, [CanBeNull] Texture2D nextTexture);
    }
}