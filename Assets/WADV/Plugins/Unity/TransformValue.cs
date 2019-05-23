using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;
using WADV.Translation;
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
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>加法互操作器</description></item>
    ///     <item><description>减法互操作器</description></item>
    ///     <item><description>相等比较互操作器</description></item>
    /// </list>
    /// </summary>
    /// <remarks>加法会将右侧有的数据覆盖到左端的副本，减法则会从左端的副本中去掉右侧有的数据项</remarks>
    [Serializable]
    public class TransformValue : SerializableValue, ISerializable, IAddOperator, ISubtractOperator, IEqualOperator {
        private readonly float[] _data = new float[21];
        private readonly bool[] _hasData = new bool[21];
        
        public TransformValue() { }
        
        protected TransformValue(SerializationInfo info, StreamingContext context) {
            _data = (float[]) info.GetValue("d", typeof(float[]));
            _hasData = (bool[]) info.GetValue("h", typeof(bool[]));
        }
        
        public override SerializableValue Clone() {
            var result = new TransformValue();
            Array.Copy(_data, result._data, 21);
            Array.Copy(_hasData, result._hasData, 21);
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
        public TransformValue Set(PropertyName name, float? value) {
            if (value.HasValue) {
                _data[(int) name] = value.Value;
                _hasData[(int) name] = true;
            } else {
                _data[(int) name] = default;
                _hasData[(int) name] = false;
            }
            return this;
        }

        /// <summary>
        /// 将此Transform属性集应用至目标Transform
        /// </summary>
        /// <param name="target">目标Transform</param>
        public void ApplyTo(Transform target) {
            if (target is RectTransform rectTransform) {
                ApplyTo(rectTransform);
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
        public void ApplyTo(RectTransform target) {
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

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is TransformValue transformValue)) throw new NotSupportedException($"Unable to add TransformValue: target {target} is not TransformValue");
            var value = transformValue.Get(PropertyName.AnchorMinX);
            var result = (TransformValue) Clone();
            if (value.HasValue) {
                result.Set(PropertyName.AnchorMinX, value);
            }
            value = transformValue.Get(PropertyName.AnchorMinY);
            if (value.HasValue) {
                result.Set(PropertyName.AnchorMinY, value);
            }
            value = transformValue.Get(PropertyName.AnchorMaxX);
            if (value.HasValue) {
                result.Set(PropertyName.AnchorMaxX, value);
            }
            value = transformValue.Get(PropertyName.AnchorMaxY);
            if (value.HasValue) {
                result.Set(PropertyName.AnchorMaxY, value);
            }
            value = transformValue.Get(PropertyName.PivotX);
            if (value.HasValue) {
                result.Set(PropertyName.PivotX, value);
            }
            value = transformValue.Get(PropertyName.PivotY);
            if (value.HasValue) {
                result.Set(PropertyName.PivotY, value);
            }
            value = transformValue.Get(PropertyName.PositionX);
            if (value.HasValue) {
                result.Set(PropertyName.PositionX, value);
            }
            value = transformValue.Get(PropertyName.PositionY);
            if (value.HasValue) {
                result.Set(PropertyName.PositionY, value);
            }
            value = transformValue.Get(PropertyName.PositionZ);
            if (value.HasValue) {
                result.Set(PropertyName.PositionZ, value);
            }
            value = transformValue.Get(PropertyName.Left);
            if (value.HasValue) {
                result.Set(PropertyName.Left, value);
            }
            value = transformValue.Get(PropertyName.Bottom);
            if (value.HasValue) {
                result.Set(PropertyName.Bottom, value);
            }
            value = transformValue.Get(PropertyName.Right);
            if (value.HasValue) {
                result.Set(PropertyName.Right, value);
            }
            value = transformValue.Get(PropertyName.Top);
            if (value.HasValue) {
                result.Set(PropertyName.Top, value);
            }
            value = transformValue.Get(PropertyName.Width);
            if (value.HasValue) {
                result.Set(PropertyName.Width, value);
            }
            value = transformValue.Get(PropertyName.Height);
            if (value.HasValue) {
                result.Set(PropertyName.Height, value);
            }
            value = transformValue.Get(PropertyName.ScaleX);
            if (value.HasValue) {
                result.Set(PropertyName.ScaleX, value);
            }
            value = transformValue.Get(PropertyName.ScaleY);
            if (value.HasValue) {
                result.Set(PropertyName.ScaleY, value);
            }
            value = transformValue.Get(PropertyName.ScaleZ);
            if (value.HasValue) {
                result.Set(PropertyName.ScaleZ, value);
            }
            value = transformValue.Get(PropertyName.RotationX);
            if (value.HasValue) {
                result.Set(PropertyName.RotationX, value);
            }
            value = transformValue.Get(PropertyName.RotationY);
            if (value.HasValue) {
                result.Set(PropertyName.RotationY, value);
            }
            value = transformValue.Get(PropertyName.RotationZ);
            if (value.HasValue) {
                result.Set(PropertyName.RotationZ, value);
            }
            return result;
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is TransformValue transformValue)) throw new NotSupportedException($"Unable to add TransformValue: target {target} is not TransformValue");
            var result = (TransformValue) Clone();
            if (transformValue.Get(PropertyName.AnchorMinX).HasValue) {
                result.Set(PropertyName.AnchorMinX, null);
            }
            if (transformValue.Get(PropertyName.AnchorMinY).HasValue) {
                result.Set(PropertyName.AnchorMinY, null);
            }
            if (transformValue.Get(PropertyName.AnchorMaxX).HasValue) {
                result.Set(PropertyName.AnchorMaxX, null);
            }
            if (transformValue.Get(PropertyName.AnchorMaxY).HasValue) {
                result.Set(PropertyName.AnchorMaxY, null);
            }
            if (transformValue.Get(PropertyName.PivotX).HasValue) {
                result.Set(PropertyName.PivotX, null);
            }
            if (transformValue.Get(PropertyName.PivotY).HasValue) {
                result.Set(PropertyName.PivotY, null);
            }
            if (transformValue.Get(PropertyName.PositionX).HasValue) {
                result.Set(PropertyName.PositionX, null);
            }
            if (transformValue.Get(PropertyName.PositionY).HasValue) {
                result.Set(PropertyName.PositionY, null);
            }
            if (transformValue.Get(PropertyName.PositionZ).HasValue) {
                result.Set(PropertyName.PositionZ, null);
            }
            if (transformValue.Get(PropertyName.Left).HasValue) {
                result.Set(PropertyName.Left, null);
            }
            if (transformValue.Get(PropertyName.Bottom).HasValue) {
                result.Set(PropertyName.Bottom, null);
            }
            if (transformValue.Get(PropertyName.Right).HasValue) {
                result.Set(PropertyName.Right, null);
            }
            if (transformValue.Get(PropertyName.Top).HasValue) {
                result.Set(PropertyName.Top, null);
            }
            if (transformValue.Get(PropertyName.Width).HasValue) {
                result.Set(PropertyName.Width, null);
            }
            if (transformValue.Get(PropertyName.Height).HasValue) {
                result.Set(PropertyName.Height, null);
            }
            if (transformValue.Get(PropertyName.ScaleX).HasValue) {
                result.Set(PropertyName.ScaleX, null);
            }
            if (transformValue.Get(PropertyName.ScaleY).HasValue) {
                result.Set(PropertyName.ScaleY, null);
            }
            if (transformValue.Get(PropertyName.ScaleZ).HasValue) {
                result.Set(PropertyName.ScaleZ, null);
            }
            if (transformValue.Get(PropertyName.RotationX).HasValue) {
                result.Set(PropertyName.RotationX, null);
            }
            if (transformValue.Get(PropertyName.RotationY).HasValue) {
                result.Set(PropertyName.RotationY, null);
            }
            if (transformValue.Get(PropertyName.RotationZ).HasValue) {
                result.Set(PropertyName.RotationZ, null);
            }
            return result;
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is TransformValue transformValue)) return false;
            for (var i = -1; ++i < 22;) {
                if (!transformValue._data[i].Equals(_data[i])) return false;
                if (transformValue._hasData[i] != _hasData[i]) return false;
            }
            return true;
        }
    }
}