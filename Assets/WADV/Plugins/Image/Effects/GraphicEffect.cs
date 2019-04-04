using System;
using System.Collections.Generic;
using System.Reflection;
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
        public float Duration { get; private set; }
        /// <summary>
        /// 缓动函数
        /// </summary>
        public Func<float, float> EasingFunction { get; private set; }
        /// <summary>
        /// 缓动函数类型
        /// </summary>
        public EasingType EasingType { get; private set; }
        /// <summary>
        /// 特效初始化参数
        /// </summary>
        public Dictionary<string, SerializableValue> Parameters { get; private set; }

        private void SetStaticData(Dictionary<string, SerializableValue> parameters, float duration, EasingType easing) {
            Parameters = parameters;
            Duration = duration;
            EasingFunction = Easing.GetEasingFunction(easing);
            EasingType = easing;
        }

        /// <summary>
        /// 根据类型创建UI效果的实例
        /// </summary>
        /// <param name="target">目标类型</param>
        /// <param name="parameters">效果参数</param>
        /// <param name="duration">持续时间</param>
        /// <param name="easing">缓动函数类型</param>
        /// <returns></returns>
        public static T CreateInstance<T>(Type target, Dictionary<string, SerializableValue> parameters, float duration, EasingType easing) where T : GraphicEffect {
            var instance = (T) Activator.CreateInstance(target);
            instance.SetStaticData(parameters, duration, easing);
            return instance;
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