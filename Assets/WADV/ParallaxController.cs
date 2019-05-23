using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WADV {
    [AddComponentMenu("Effects/Parallax Controller")]
    public class ParallaxController : MonoBehaviour {
        [HideInInspector]
        public List<ParallaxTarget> targets;
        public new Camera camera;

        private readonly Dictionary<RectTransform, Vector3> _cache = new Dictionary<RectTransform, Vector3>();
        private Vector3 _lastMousePosition;

        private void Start() {
            foreach (var target in targets) {
                _cache.Add(target.transform, target.transform.position);
            }
        }

        private void Update() {
            if (Input.mousePosition == _lastMousePosition || !targets.Any()) return;
            _lastMousePosition = Input.mousePosition;
            var mouse = camera.ScreenToViewportPoint(_lastMousePosition);
            mouse = new Vector3(mouse.x - 0.5F, mouse.y - 0.5F, mouse.z);
            var length = targets.Count;
            for (var i = -1; ++i < length;) {
                var target = targets[i];
                var cache = _cache[target.transform];
                target.transform.position = new Vector3(cache.x + mouse.x * target.scale, cache.y + mouse.y * target.scale, cache.z);
            }
        }

        /// <summary>
        /// 添加或更新现有的一个视差操作对象
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="scale">视差移动等级，越大越明显</param>
        public void Add(RectTransform target, int scale) {
            if (_cache.ContainsKey(target)) {
                targets.RemoveAt(targets.FindIndex(e => e.transform == target));
                _cache.Remove(target);
            }
            _cache.Add(target, target.position);
            targets.Add(new ParallaxTarget {transform = target, scale = scale});
        }

        /// <summary>
        /// 移除一个视差操作对象
        /// </summary>
        /// <param name="target">目标对象</param>
        public void Remove(RectTransform target) {
            if (!_cache.ContainsKey(target)) return;
            _cache.Remove(target);
            targets.RemoveAll(e => e.transform == target);
        }

        [Serializable]
        public struct ParallaxTarget {
            public RectTransform transform;
            public int scale;
        }
    }
}