using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Plugins.Vector;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Unity {
    [StaticRegistrationInfo("TransformProperty")]
    [UsedImplicitly]
    public class TransformPropertyPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var transformProperty = new TransformPropertyValue();
            foreach (var (key, value) in context.StringParameters) {
                float? x, y, z;
                switch (key.ConvertToString(context.Language)) {
                    case "AnchorMinX":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, FloatValue.TryParse(value));
                        break;
                    case "AnchorMinY":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, FloatValue.TryParse(value));
                        break;
                    case "AnchorMin":
                        (x, y, _) = GetVectorComponents(value, context.Language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, y);
                        }
                        break;
                    case "AnchorMaxX":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, FloatValue.TryParse(value));
                        break;
                    case "AnchorMaxY":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, FloatValue.TryParse(value));
                        break;
                    case "AnchorMax":
                        (x, y, _) = GetVectorComponents(value, context.Language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, y);
                        }
                        break;
                    case "PositionX":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PositionX, FloatValue.TryParse(value));
                        break;
                    case "PositionY":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PositionY, FloatValue.TryParse(value));
                        break;
                    case "PositionZ":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PositionZ, FloatValue.TryParse(value));
                        break;
                    case "Position":
                        (x, y, z) = GetVectorComponents(value, context.Language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.PositionX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.PositionY, y);
                        }
                        if (z.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.PositionZ, z);
                        }
                        break;
                    case "Width":
                        transformProperty.Set(TransformPropertyValue.PropertyName.Width, FloatValue.TryParse(value));
                        break;
                    case "Height":
                        transformProperty.Set(TransformPropertyValue.PropertyName.Height, FloatValue.TryParse(value));
                        break;
                    case "Top":
                        transformProperty.Set(TransformPropertyValue.PropertyName.Top, FloatValue.TryParse(value));
                        break;
                    case "Bottom":
                        transformProperty.Set(TransformPropertyValue.PropertyName.Bottom, FloatValue.TryParse(value));
                        break;
                    case "Left":
                        transformProperty.Set(TransformPropertyValue.PropertyName.Left, FloatValue.TryParse(value));
                        break;
                    case "Right":
                        transformProperty.Set(TransformPropertyValue.PropertyName.Right, FloatValue.TryParse(value));
                        break;
                    case "PivotX":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, FloatValue.TryParse(value));
                        break;
                    case "PivotY":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, FloatValue.TryParse(value));
                        break;
                    case "Pivot":
                        (x, y, _) = GetVectorComponents(value, context.Language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, y);
                        }
                        break;
                    case "ScaleX":
                        transformProperty.Set(TransformPropertyValue.PropertyName.ScaleX, FloatValue.TryParse(value));
                        break;
                    case "ScaleY":
                        transformProperty.Set(TransformPropertyValue.PropertyName.ScaleY, FloatValue.TryParse(value));
                        break;
                    case "ScaleZ":
                        transformProperty.Set(TransformPropertyValue.PropertyName.ScaleZ, FloatValue.TryParse(value));
                        break;
                    case "Scale":
                        (x, y, z) = GetVectorComponents(value, context.Language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.ScaleX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.ScaleY, y);
                        }
                        if (z.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.ScaleZ, z);
                        }
                        break;
                    case "RotationX":
                        transformProperty.Set(TransformPropertyValue.PropertyName.RotationX, FloatValue.TryParse(value));
                        break;
                    case "RotationY":
                        transformProperty.Set(TransformPropertyValue.PropertyName.RotationY, FloatValue.TryParse(value));
                        break;
                    case "RotationZ":
                        transformProperty.Set(TransformPropertyValue.PropertyName.RotationZ, FloatValue.TryParse(value));
                        break;
                    case "Rotation":
                        (x, y, z) = GetVectorComponents(value, context.Language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.RotationX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.RotationY, y);
                        }
                        if (z.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.RotationZ, z);
                        }
                        break;
                    case "AnchorTopLeft":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorTopCenter":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorTopRight":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorCenterLeft":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 0.5F);
                        break;
                    case "AnchorCenter":
                    case "AnchorCenterCenter":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 0.5F);
                        break;
                    case "AnchorCenterRight":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 0.5F);
                        break;
                    case "AnchorBottomLeft":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 0.0F);
                        break;
                    case "AnchorBottomCenter":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 0.0F);
                        break;
                    case "AnchorBottomRight":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 0.0F);
                        break;
                    case "AnchorTopAuto":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorHorizontalAuto":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 0.5F);
                        break;
                    case "AnchorBottomAuto":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 0.0F);
                        break;
                    case "AnchorLeftAuto":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorVerticalAuto":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorRightAuto":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorAuto":
                    case "AnchorAllAuto":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "PivotCenter":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, 0.5F);
                        break;
                    case "PivotTopLeft":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, 1.0F);
                        break;
                    case "PivotTopCenter":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, 1.0F);
                        break;
                    case "PivotTopRight":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, 1.0F);
                        break;
                    case "PivotBottomLeft":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, 0.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, 0.0F);
                        break;
                    case "PivotBottomCenter":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, 0.5F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, 0.0F);
                        break;
                    case "PivotBottomRight":
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotX, 1.0F);
                        transformProperty.Set(TransformPropertyValue.PropertyName.PivotY, 0.0F);
                        break;
                }
            }
            return Task.FromResult<SerializableValue>(transformProperty);
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }

        private static (float? x, float? y, float? z) GetVectorComponents(SerializableValue source, string language) {
            switch (source) {
                case Vector2Value vector2Value:
                    return (vector2Value.value.x, vector2Value.value.y, null);
                case Vector3Value vector3Value:
                    return (vector3Value.value.x, vector3Value.value.y, vector3Value.value.z);
                case IFloatConverter floatConverter:
                    var floatValue = floatConverter.ConvertToFloat();
                    return (floatValue, floatValue, floatValue);
                case IIntegerConverter integerConverter:
                    var intValue = integerConverter.ConvertToInteger();
                    return (intValue, intValue, intValue);
                default:
                    return (null, null, null);
            }
        }
    }
}