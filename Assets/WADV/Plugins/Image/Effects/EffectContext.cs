using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WADV.Plugins.Image.Effects {
    /// <summary>
    /// 效果播放上下文
    /// </summary>
    public class EffectContext {
        /// <summary>
        /// 效果对象
        /// </summary>
        public IEnumerable<Graphic> graphics;
        
        /// <summary>
        /// 目标材质
        /// </summary>
        [CanBeNull] public Texture2D targetTexture;

        private readonly float _time;
        [CanBeNull] private readonly Func<float, float> _easing;

        /// <summary>
        /// 创建一个效果播放上下文
        /// </summary>
        /// <param name="time">总播放时间</param>
        /// <param name="easing">缓动函数</param>
        public EffectContext(float time, Func<float, float> easing = null) {
            _time = time;
            _easing = easing;
        }
        
        /// <summary>
        /// 创建一个效果播放上下文
        /// </summary>
        /// <param name="time">总播放时间</param>
        /// <param name="easing">缓动类型</param>
        public EffectContext(float time, EasingType easing = EasingType.Linear) {
            _time = time;
            _easing = easing == EasingType.Linear ? null : Easing.GetEasingFunction(easing);
        }
        
        /// <summary>
        /// 获取播放进度
        /// </summary>
        /// <param name="time">自播放开始经过的时间</param>
        /// <returns></returns>
        public float GetProgress(float time) => Mathf.Clamp01(_easing?.Invoke(_time) / _time ?? _time / _time);
    }
}