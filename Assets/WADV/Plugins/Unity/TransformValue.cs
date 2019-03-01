using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Unity {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="ISerializable" />
    /// <summary>
    /// <para>表示一个RectTransform的基本属性集合</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>值复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>105 字节</description></item>
    ///     <item><description>2 数组信息</description></item>
    ///     <item><description>2 名称长度1的SerializationInfo项目</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class TransformValue : SerializableValue, ISerializable {
        private readonly float[] _data = new float[21];
        private readonly bool[] _hasData = new bool[21];
        
        public TransformValue() { }
        
        protected TransformValue(SerializationInfo info, StreamingContext context) {
            _data = (float[]) info.GetValue("d", typeof(float[]));
            _hasData = (bool[]) info.GetValue("h", typeof(bool[]));
        }
        
        public override SerializableValue Duplicate() {
            var result = new TransformValue();
            for (var i = -1; ++i < 22;) {
                result._data[i] = _data[i];
                result._hasData[i] = _hasData[i];
            }
            return result;
        }
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("d", _data);
            info.AddValue("h", _hasData);
        }

        /// <summary>
        /// 获取Transform属性值
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <returns></returns>
        public float? Get(PropertyName name) {
            return _hasData[(int) name] ? (float?) _data[(int) name] : null;
        }

        /// <summary>
        /// 设置Transform属性值
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <param name="value">属性值</param>
        public void Set(PropertyName name, float? value) {
            if (value.HasValue) {
                _data[(int) name] = value.Value;
                _hasData[(int) name] = true;
            } else {
                _data[(int) name] = default(float);
                _hasData[(int) name] = false;
            }
        }

        /// <summary>
        /// 将此Transform属性集应用至目标Transform
        /// </summary>
        /// <param name="target">目标Transform</param>
        public void ApplyTo(ref Transform target) {
            if (target is RectTransform rectTransform) {
                ApplyTo(ref rectTransform);
                return;
            }
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
            if (x.HasValue || y.HasValue || z.HasValue) {
                var data = target.localRotation;
                var rotation = data.eulerAngles;
                target.localRotation = Quaternion.Euler(x ?? rotation.x, y ?? rotation.y, z ?? rotation.z);
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
        /// <para>属性按照AnchorMin->AnchorMax->Pivot->Position->Left/Bottom->Right/Top->Width/Height->Scale->Rotation的顺序应用，由于定位并不需要所有属性，某些组合可能引发预期外的效果</para>
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
            if (x.HasValue || y.HasValue || z.HasValue) {
                var data = target.localRotation;
                var rotation = data.eulerAngles;
                target.localRotation = Quaternion.Euler(x ?? rotation.x, y ?? rotation.y, z ?? rotation.z);
            }
        }

        /// <summary>
        /// Transform属性名称
        /// </summary>
        public enum PropertyName {
            /// <summary>
            /// 锚区左下角X坐标
            /// </summary>
            AnchorMinX,
            /// <summary>
            /// 锚区左下角Y坐标
            /// </summary>
            AnchorMinY,
            /// <summary>
            /// 锚区右上角X坐标
            /// </summary>
            AnchorMaxX,
            /// <summary>
            /// 锚区右上角Y坐标
            /// </summary>
            AnchorMaxY,
            /// <summary>
            /// 变换基础点X坐标
            /// </summary>
            PivotX,
            /// <summary>
            /// 变换基础点Y坐标
            /// </summary>
            PivotY,
            /// <summary>
            /// 位置X坐标
            /// </summary>
            PositionX,
            /// <summary>
            /// 位置Y坐标
            /// </summary>
            PositionY,
            /// <summary>
            /// 位置Z坐标
            /// </summary>
            PositionZ,
            /// <summary>
            /// 与锚区左侧的距离
            /// </summary>
            Left,
            /// <summary>
            /// 与锚区右侧的距离
            /// </summary>
            Right,
            /// <summary>
            /// 与锚区顶部的距离
            /// </summary>
            Top,
            /// <summary>
            /// 与锚区底部的距离
            /// </summary>
            Bottom,
            /// <summary>
            /// 宽度
            /// </summary>
            Width,
            /// <summary>
            /// 高度
            /// </summary>
            Height,
            /// <summary>
            /// X坐标缩放比例
            /// </summary>
            ScaleX,
            /// <summary>
            /// Y坐标缩放比例
            /// </summary>
            ScaleY,
            /// <summary>
            /// Z坐标缩放比例
            /// </summary>
            ScaleZ,
            /// <summary>
            /// X轴旋转角度
            /// </summary>
            RotationX,
            /// <summary>
            /// Y轴旋转角度
            /// </summary>
            RotationY,
            /// <summary>
            /// Z轴旋转角度
            /// </summary>
            RotationZ
        }
    }
}