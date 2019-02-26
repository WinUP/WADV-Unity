using System;
using System.Runtime.Serialization;
using UnityEngine;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Unity {
    /// <inheritdoc />
    /// <summary>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>88 字节</description></item>
    ///     <item><description>1 数组信息</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class TransformPropertyValue : SerializableValue, ISerializable {
        private readonly float?[] _data = new float?[22];
        
        public TransformPropertyValue() { }
        
        protected TransformPropertyValue(SerializationInfo info, StreamingContext context) {
            _data = (float?[]) info.GetValue("data", typeof(float?[]));
        }
        
        public override SerializableValue Duplicate() {
            var result = new TransformPropertyValue();
            for (var i = -1; ++i < 22;) {
                result._data[i] = _data[i];
            }
            return result;
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("data", _data);
        }

        /// <summary>
        /// 获取Transform属性值
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <returns></returns>
        public float? Get(PropertyName name) {
            return _data[(int) name];
        }

        /// <summary>
        /// 将此Transform属性集应用至目标Transform
        /// </summary>
        /// <param name="target">目标Transform</param>
        public void ApplyTo(ref Transform target) {
            var x = Get(PropertyName.ScaleX);
            var y = Get(PropertyName.ScaleY);
            var z = Get(PropertyName.ScaleZ);
            if (x.HasValue || y.HasValue || z.HasValue) {
                var data = target.localScale;
                target.localScale = new Vector3(x ?? data.x, y ?? data.y, z ?? data.z);
            }
            x = Get(PropertyName.RotationX);
            y = Get(PropertyName.RotationY);
            z = Get(PropertyName.RotationZ);
            var w = Get(PropertyName.RotationW);
            if (x.HasValue || y.HasValue || z.HasValue || w.HasValue) {
                var data = target.localRotation;
                target.localRotation = new Quaternion(x ?? data.x, y ?? data.y, z ?? data.z, w ?? data.w);
            }
            x = Get(PropertyName.PositionX);
            y = Get(PropertyName.PositionY);
            z = Get(PropertyName.PositionZ);
            if (x.HasValue || y.HasValue || z.HasValue) {
                var data = target.localPosition;
                target.localPosition = new Vector3(x ?? data.x, y ?? data.y, z ?? data.z);
            }
        }

        /// <summary>
        /// 将此Transform属性集应用至目标RectTransform
        /// </summary>
        /// <param name="target">目标RectTransform</param>
        public void ApplyTo(ref RectTransform target) {
            var x = Get(PropertyName.AnchorMinX);
            var y = Get(PropertyName.AnchorMinY);
            if (x.HasValue || y.HasValue) {
                var data = target.anchorMin;
                target.anchorMin = new Vector2(x ?? data.x, y ?? data.y);
            }
            x = Get(PropertyName.AnchorMaxX);
            y = Get(PropertyName.AnchorMaxY);
            if (x.HasValue || y.HasValue) {
                var data = target.anchorMin;
                target.anchorMax = new Vector2(x ?? data.x, y ?? data.y);
            }
            x = Get(PropertyName.PivotX);
            y = Get(PropertyName.PivotY);
            if (x.HasValue || y.HasValue) {
                var data = target.anchorMin;
                target.pivot = new Vector2(x ?? data.x, y ?? data.y);
            }
            x = Get(PropertyName.PositionX);
            y = Get(PropertyName.PositionY);
            var z = Get(PropertyName.PositionZ);
            if (x.HasValue || y.HasValue || z.HasValue) {
                var data = target.anchoredPosition3D;
                target.anchoredPosition3D = new Vector3(x ?? data.x, y ?? data.y, z ?? data.z);
            }
            x = Get(PropertyName.Left);
            y = Get(PropertyName.Bottom);
            if (x.HasValue || y.HasValue) {
                var data = target.offsetMin;
                target.offsetMin = new Vector2(x ?? data.x, y ?? data.y);
            }
            x = Get(PropertyName.Right);
            y = Get(PropertyName.Top);
            if (x.HasValue || y.HasValue) {
                var data = target.offsetMax;
                target.offsetMax = new Vector2(-x ?? data.x, -y ?? data.y);
            }
            x = Get(PropertyName.Width);
            y = Get(PropertyName.Height);
            if (x.HasValue || y.HasValue) {
                var data = target.sizeDelta;
                target.sizeDelta = new Vector2(x ?? data.x, y ?? data.y);
            }
            x = Get(PropertyName.ScaleX);
            y = Get(PropertyName.ScaleY);
            z = Get(PropertyName.ScaleZ);
            if (x.HasValue || y.HasValue || z.HasValue) {
                var data = target.localScale;
                target.localScale = new Vector3(x ?? data.x, y ?? data.y, z ?? data.z);
            }
            x = Get(PropertyName.RotationX);
            y = Get(PropertyName.RotationY);
            z = Get(PropertyName.RotationZ);
            var w = Get(PropertyName.RotationW);
            if (x.HasValue || y.HasValue || z.HasValue || w.HasValue) {
                var data = target.localRotation;
                target.localRotation = new Quaternion(x ?? data.x, y ?? data.y, z ?? data.z, w ?? data.w);
            }
        }

        public enum PropertyName {
            AnchorMinX,
            AnchorMinY,
            AnchorMaxX,
            AnchorMaxY,
            PivotX,
            PivotY,
            PositionX,
            PositionY,
            PositionZ,
            Left,
            Right,
            Top,
            Bottom,
            Width,
            Height,
            ScaleX,
            ScaleY,
            ScaleZ,
            RotationX,
            RotationY,
            RotationZ,
            RotationW
        }
    }
}