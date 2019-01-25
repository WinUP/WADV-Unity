using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.Extensions;
using WADV.Thread;

namespace WADV {
    /// <inheritdoc />
    /// <summary>
    /// UGUI颜色渐变组件
    /// </summary>
    [AddComponentMenu("UI/Effects/Color Fade")]
    [DisallowMultipleComponent]
    public class GraphicColorFade : MonoBehaviour {
        /// <summary>
        /// 起始颜色
        /// </summary>
        public Color startColor = new Color(1.0F, 1.0F, 1.0F, 1.0F);
        
        /// <summary>
        /// 终止颜色
        /// </summary>
        public Color targetColor = new Color(1.0F, 1.0F, 1.0F, 0.0F);

        /// <summary>
        /// 渐变缓动类型
        /// </summary>
        public EasingType easingType = EasingType.Linear;

        /// <summary>
        /// 渐变时间
        /// </summary>
        public float fadeTime = 0.1F;
        
        [HideInInspector]
        public List<Graphic> targets;

        private void Start() {
            foreach (var target in targets) {
                target.color = startColor;
            }
        }

        public async Task Fade() {
            var passedTime = 0.0F;
            var easing = Easing.GetEasingFunction(easingType);
            while (passedTime < fadeTime) {
                passedTime += Time.deltaTime;
                ApplyColor(ref startColor, ref targetColor, easing(passedTime / fadeTime));
                await Dispatcher.NextUpdate();
            }
            
        }

        public void FadeFrame(float progress) {
            ApplyColor(ref startColor, ref targetColor, Easing.GetEasingFunction(easingType)(progress));
        }

        public async Task Reverse() {
            var passedTime = 0.0F;
            var easing = Easing.GetEasingFunction(easingType);
            while (passedTime < fadeTime) {
                passedTime += Time.deltaTime;
                ApplyColor(ref targetColor, ref startColor, easing(passedTime / fadeTime));
                await Dispatcher.NextUpdate();
            }
        }
        
        public void ReverseFrame(float progress) {
            ApplyColor(ref targetColor, ref startColor, Easing.GetEasingFunction(easingType)(progress));
        }
        
        private void ApplyColor(ref Color from, ref Color to, float progress) {
            foreach (var target in targets) {
                target.color = new Color(
                    Mathf.Lerp(from.r, to.r, progress),
                    Mathf.Lerp(from.g, to.g, progress),
                    Mathf.Lerp(from.b, to.b, progress),
                    Mathf.Lerp(from.a, to.a, progress));
            }
        }
    }
}