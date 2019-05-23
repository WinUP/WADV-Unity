using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Unity {
    [StaticRegistrationInfo("Transform")]
    [UsedImplicitly]
    public class TransformPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var transformProperty = new TransformValue();
            foreach (var (key, value) in context.StringParameters) {
                AnalyzePropertyTo(key.ConvertToString(context.Language), value, transformProperty, context.Language);
            }
            return Task.FromResult<SerializableValue>(transformProperty);
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }

        public static void AnalyzePropertyTo(string key, SerializableValue value, TransformValue transformProperty, string language) {
            float? x, y, z;
                switch (key) {
                    case "AnchorMinX":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, FloatValue.TryParse(value, language));
                        break;
                    case "AnchorMinY":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, FloatValue.TryParse(value, language));
                        break;
                    case "AnchorMin":
                        (x, y, _) = GetVectorComponents(value, language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.AnchorMinX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.AnchorMinY, y);
                        }
                        break;
                    case "AnchorMaxX":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, FloatValue.TryParse(value, language));
                        break;
                    case "AnchorMaxY":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, FloatValue.TryParse(value, language));
                        break;
                    case "AnchorMax":
                        (x, y, _) = GetVectorComponents(value, language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, y);
                        }
                        break;
                    case "PositionX":
                        transformProperty.Set(TransformValue.PropertyName.PositionX, FloatValue.TryParse(value, language));
                        break;
                    case "PositionY":
                        transformProperty.Set(TransformValue.PropertyName.PositionY, FloatValue.TryParse(value, language));
                        break;
                    case "PositionZ":
                        transformProperty.Set(TransformValue.PropertyName.PositionZ, FloatValue.TryParse(value, language));
                        break;
                    case "Position":
                        (x, y, z) = GetVectorComponents(value, language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.PositionX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.PositionY, y);
                        }
                        if (z.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.PositionZ, z);
                        }
                        break;
                    case "Width":
                        transformProperty.Set(TransformValue.PropertyName.Width, FloatValue.TryParse(value, language));
                        break;
                    case "Height":
                        transformProperty.Set(TransformValue.PropertyName.Height, FloatValue.TryParse(value, language));
                        break;
                    case "Top":
                        transformProperty.Set(TransformValue.PropertyName.Top, FloatValue.TryParse(value, language));
                        break;
                    case "Bottom":
                        transformProperty.Set(TransformValue.PropertyName.Bottom, FloatValue.TryParse(value, language));
                        break;
                    case "Left":
                        transformProperty.Set(TransformValue.PropertyName.Left, FloatValue.TryParse(value, language));
                        break;
                    case "Right":
                        transformProperty.Set(TransformValue.PropertyName.Right, FloatValue.TryParse(value, language));
                        break;
                    case "PivotX":
                        transformProperty.Set(TransformValue.PropertyName.PivotX, FloatValue.TryParse(value, language));
                        break;
                    case "PivotY":
                        transformProperty.Set(TransformValue.PropertyName.PivotY, FloatValue.TryParse(value, language));
                        break;
                    case "Pivot":
                        (x, y, _) = GetVectorComponents(value, language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.PivotX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.PivotY, y);
                        }
                        break;
                    case "ScaleX":
                        transformProperty.Set(TransformValue.PropertyName.ScaleX, FloatValue.TryParse(value, language));
                        break;
                    case "ScaleY":
                        transformProperty.Set(TransformValue.PropertyName.ScaleY, FloatValue.TryParse(value, language));
                        break;
                    case "ScaleZ":
                        transformProperty.Set(TransformValue.PropertyName.ScaleZ, FloatValue.TryParse(value, language));
                        break;
                    case "Scale":
                        (x, y, z) = GetVectorComponents(value, language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.ScaleX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.ScaleY, y);
                        }
                        if (z.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.ScaleZ, z);
                        }
                        break;
                    case "RotationX":
                        transformProperty.Set(TransformValue.PropertyName.RotationX, FloatValue.TryParse(value, language));
                        break;
                    case "RotationY":
                        transformProperty.Set(TransformValue.PropertyName.RotationY, FloatValue.TryParse(value, language));
                        break;
                    case "RotationZ":
                        transformProperty.Set(TransformValue.PropertyName.RotationZ, FloatValue.TryParse(value, language));
                        break;
                    case "Rotation":
                        (x, y, z) = GetVectorComponents(value, language);
                        if (x.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.RotationX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.RotationY, y);
                        }
                        if (z.HasValue) {
                            transformProperty.Set(TransformValue.PropertyName.RotationZ, z);
                        }
                        break;
                    case "AnchorTopLeft":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorTopCenter":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorTopRight":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorCenterLeft":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 0.5F);
                        break;
                    case "AnchorCenter":
                    case "AnchorCenterCenter":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 0.5F);
                        break;
                    case "AnchorCenterRight":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 0.5F);
                        break;
                    case "AnchorBottomLeft":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 0.0F);
                        break;
                    case "AnchorBottomCenter":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 0.0F);
                        break;
                    case "AnchorBottomRight":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 0.0F);
                        break;
                    case "AnchorTopAuto":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorHorizontalAuto":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 0.5F);
                        break;
                    case "AnchorBottomAuto":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 0.0F);
                        break;
                    case "AnchorLeftAuto":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorVerticalAuto":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorRightAuto":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "AnchorAuto":
                    case "AnchorAllAuto":
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMinY, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.AnchorMaxY, 1.0F);
                        break;
                    case "PivotCenter":
                        transformProperty.Set(TransformValue.PropertyName.PivotX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.PivotY, 0.5F);
                        break;
                    case "PivotTopLeft":
                        transformProperty.Set(TransformValue.PropertyName.PivotX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.PivotY, 1.0F);
                        break;
                    case "PivotTopCenter":
                        transformProperty.Set(TransformValue.PropertyName.PivotX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.PivotY, 1.0F);
                        break;
                    case "PivotTopRight":
                        transformProperty.Set(TransformValue.PropertyName.PivotX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.PivotY, 1.0F);
                        break;
                    case "PivotBottomLeft":
                        transformProperty.Set(TransformValue.PropertyName.PivotX, 0.0F);
                        transformProperty.Set(TransformValue.PropertyName.PivotY, 0.0F);
                        break;
                    case "PivotBottomCenter":
                        transformProperty.Set(TransformValue.PropertyName.PivotX, 0.5F);
                        transformProperty.Set(TransformValue.PropertyName.PivotY, 0.0F);
                        break;
                    case "PivotBottomRight":
                        transformProperty.Set(TransformValue.PropertyName.PivotX, 1.0F);
                        transformProperty.Set(TransformValue.PropertyName.PivotY, 0.0F);
                        break;
                }
        }

        private static (float? x, float? y, float? z) GetVectorComponents(SerializableValue source, string language) {
            switch (source) {
                case Vector2Value vector2Value:
                    return (vector2Value.value.x, vector2Value.value.y, null);
                case Vector3Value vector3Value:
                    return (vector3Value.value.x, vector3Value.value.y, vector3Value.value.z);
                case IFloatConverter floatConverter:
                    var floatValue = floatConverter.ConvertToFloat(language);
                    return (floatValue, floatValue, floatValue);
                case IIntegerConverter integerConverter:
                    var intValue = integerConverter.ConvertToInteger(language);
                    return (intValue, intValue, intValue);
                default:
                    return (null, null, null);
            }
        }
    }
}