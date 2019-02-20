using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public abstract class GraphicEffect {
        protected readonly float Duration;
        protected readonly Func<float, float> EasingFunction;
        protected readonly EasingType EasingType;
        protected readonly Dictionary<string, SerializableValue> Parameters;

        /// <summary>
        /// 创建一个效果播放上下文
        /// </summary>
        /// <param name="parameters">效果参数</param>
        /// <param name="duration">总播放时间</param>
        /// <param name="easing">缓动类型</param>
        public GraphicEffect(Dictionary<string, SerializableValue> parameters, float duration, EasingType easing = EasingType.Linear) {
            Parameters = parameters;
            Duration = duration;
            EasingFunction = WADV.Easing.GetEasingFunction(easing);
            EasingType = easing;
        }

        public (float Duration, EasingType Easing, Dictionary<string, SerializableValue> Parameters) GetParameters() => (Duration, EasingType, Parameters);

        /// <summary>
        /// 获取播放进度
        /// </summary>
        /// <param name="time">自播放开始经过的时间</param>
        /// <returns></returns>
        public float GetProgress(float time) => Mathf.Clamp01(EasingFunction(time / Duration));
    }
}