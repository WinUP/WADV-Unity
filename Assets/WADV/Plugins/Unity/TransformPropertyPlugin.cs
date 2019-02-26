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
                /*
                 AnchorMinX=0 AnchorMinY=0 AnchorMin=[Vector2 X=0 Y=0]
                 AnchorMaxX=0 AnchorMaxY=0 AnchorMax=[Vector2 X=0 Y=0]
                 PositionX=0 PositionY=0 PositionZ=0 Position=[Vector2 X=0 Y=0] Position=[Vector3 X=0 Y=0 Z=0]
                 Width=800 Height=600 Left=0 Right=0 Top=0 Bottom=0
                 PivotX=0 PivotY=0 Pivot=[Vector2 X=0 Y=0]
                 ScaleX=1 ScaleY=1 ScaleZ=0 Scale=[Vector2 X=1 Y=1] Scale=[Vector3 X=1 Y=1 Z=1]
                 RotateX=0 RotateY=0 RotateZ=0 Rotate=[Vector2 X=0 Y=0] Rotate=[Vector3 X=0 Y=0 Z=0]
                 */
                switch (key.ConvertToString(context.Language)) {
                    case "AnchorMinX":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, FloatValue.TryParse(value));
                        break;
                    case "AnchorMinY":
                        transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, FloatValue.TryParse(value));
                        break;
                    case "AnchorMin":
                        var (x, y, _) = GetVectorComponents(value);
                        if (x.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinX, x);
                        }
                        if (y.HasValue) {
                            transformProperty.Set(TransformPropertyValue.PropertyName.AnchorMinY, y);
                        }
                }
            }
            
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }

        private (float? x, float? y, float? z) GetVectorComponents(SerializableValue source) {
            switch (source) {
                case Vector2Value vector2Value:
                    return (vector2Value.value.x, vector2Value.value.y, null);
                case Vector3Value vector3Value:
                    return (vector3Value.value.x, vector3Value.value.y, vector3Value.value.z);
                default:
                    return (null, null, null);
            }
        }
    }
}