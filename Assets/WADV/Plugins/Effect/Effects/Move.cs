using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using WADV.Extensions;
using WADV.Plugins.Unity;
using WADV.Reflection;
using WADV.Thread;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Effect.Effects {
    [StaticRegistrationInfo("Move")]
    [UsedImplicitly]
    public class Move : SingleGraphicEffect {
        private Vector3 _offset = Vector3.zero;

        public override Task Initialize() {
            foreach (var (key, value) in Parameters) {
                float number;
                switch (key) {
                    case "Offset":
                        switch (value) {
                            case Vector3Value vector3:
                                _offset = vector3.value;
                                break;
                            case Vector2Value vector2:
                                _offset = vector2.value;
                                break;
                            default:
                                number = FloatValue.TryParse(value);
                                _offset = new Vector3(number, number, number);
                                break;
                        }
                        break;
                    case "X":
                        number = FloatValue.TryParse(value);
                        _offset = new Vector3(number, _offset.y, _offset.z);
                        break;
                    case "Y":
                        number = FloatValue.TryParse(value);
                        _offset = new Vector3(_offset.x, number, _offset.z);
                        break;
                    case "Z":
                        number = FloatValue.TryParse(value);
                        _offset = new Vector3(_offset.x, _offset.y, number);
                        break;
                }
            }
            return Task.CompletedTask;
        }

        public override async Task PlayEffect(IEnumerable<Graphic> targets, Texture2D nextTexture) {
            var transforms = targets.Select(e => {
                var transform = e.rectTransform;
                if (transform == null) {
                    throw new NotSupportedException($"Unable to apply move effect: {e} is not UnityUI element");
                }
                return (Transform: transform, OriginalPoint: transform.anchoredPosition3D);
            }).ToArray();
            var currentTime = 0.0F;
            while (currentTime < Duration) {
                foreach (var (transform, originalPoint) in transforms) {
                    transform.anchoredPosition3D = originalPoint + Vector3.Lerp(Vector3.zero, _offset, GetProgress(currentTime));
                }
                await Dispatcher.NextUpdate();
                currentTime += Time.deltaTime;
            }
            foreach (var (transform, originalPoint) in transforms) {
                transform.anchoredPosition3D = originalPoint + _offset;
            }
        }
    }
}