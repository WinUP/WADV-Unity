using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV;
using WADV.Extensions;
using WADV.Thread;

namespace Game.UI {
    [RequireComponent(typeof(Image))]
    public class FadeImage : MonoBehaviour {
        public Color visibleColor = new Color(1.0F, 1.0F, 1.0F, 1.0F);
        public Color hiddenColor = new Color(1.0F, 1.0F, 1.0F, 0.0F);

        public EasingType easingType = EasingType.Linear;

        public float time = 0.1F;
        
        private Image _image;

        private void Start() {
            _image = GetComponent<Image>();
            if (_image == null) throw new NotSupportedException($"Unable to create {nameof(FadeImage)}: no Image component found in current object");
        }

        public async Task ShowImage() {
            var passedTime = 0.0F;
            var easing = Easing.GetEasingFunction(easingType);
            while (passedTime < time) {
                passedTime += Time.deltaTime;
                ApplyColor(ref hiddenColor, ref visibleColor, easing(passedTime / time));
                await Dispatcher.NextUpdate();
            }
        }

        public async Task HideImage() {
            var passedTime = 0.0F;
            var easing = Easing.GetEasingFunction(easingType);
            while (passedTime < time) {
                passedTime += Time.deltaTime;
                ApplyColor(ref visibleColor, ref hiddenColor, easing(passedTime / time));
                await Dispatcher.NextUpdate();
            }
        }
        
        private void ApplyColor(ref Color from, ref Color to, float progress) {
            _image.color = new Color(
                Mathf.Lerp(from.r, to.r, progress),
                Mathf.Lerp(from.g, to.g, progress),
                Mathf.Lerp(from.b, to.b, progress),
                Mathf.Lerp(from.a, to.a, progress));
        }
    }
}