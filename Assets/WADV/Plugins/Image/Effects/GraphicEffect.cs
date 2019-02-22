using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    /// <summary>
    /// 表示一个UI特效
    /// </summary>
    public abstract class GraphicEffect {
        /// <summary>
        /// 持续时间
        /// </summary>
        public readonly float Duration;
        /// <summary>
        /// 缓动函数
        /// </summary>
        public readonly Func<float, float> EasingFunction;
        /// <summary>
        /// 缓动函数类型
        /// </summary>
        public readonly EasingType EasingType;
        /// <summary>
        /// 特效初始化参数
        /// </summary>
        public readonly Dictionary<string, SerializableValue> Parameters;

        /// <summary>
        /// 创建一个UI特效
        /// </summary>
        /// <param name="parameters">特效参数</param>
        /// <param name="duration">持续时间</param>
        /// <param name="easing">缓动函数类型</param>
        [UsedImplicitly]
        private GraphicEffect(Dictionary<string, SerializableValue> parameters, float duration, EasingType easing = EasingType.Linear) {
            Parameters = parameters;
            Duration = duration;
            EasingFunction = Easing.GetEasingFunction(easing);
            EasingType = easing;
        }
        
        protected GraphicEffect() { }

        /// <summary>
        /// 根据类型创建UI效果的实例
        /// </summary>
        /// <param name="target">目标类型</param>
        /// <param name="parameters">效果参数</param>
        /// <param name="duration">持续时间</param>
        /// <param name="easing">缓动函数类型</param>
        /// <returns></returns>
        [CanBeNull]
        public static T CreateInstance<T>(Type target, Dictionary<string, SerializableValue> parameters, float duration, EasingType easing) where T : GraphicEffect {
            try {
                return (T) Activator.CreateInstance(target, parameters, duration, easing);
            } catch {
                return null;
            }
        }

        /// <summary>
        /// 初始化特效
        /// </summary>
        /// <returns></returns>
        public virtual Task Initialize() {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取播放进度（0至1）
        /// </summary>
        /// <param name="time">自播放开始经过的时间</param>
        /// <returns></returns>
        protected float GetProgress(float time) => Mathf.Clamp01(EasingFunction(time / Duration));
    }
}