using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Vector2 {
    [Serializable]
    public class Vector2Value : SerializableValue, ISerializable {
        public UnityEngine.Vector2 Value { get; set; }
        
        public Vector2Value() { }
        
        [UsedImplicitly]
        protected Vector2Value(SerializationInfo info, StreamingContext context) {
            Value = new UnityEngine.Vector2 {x = info.GetSingle("x"), y = info.GetSingle("y")};
        }
        
        public override SerializableValue Duplicate() {
            return new Vector2Value {Value = new UnityEngine.Vector2 {x = Value.x, y = Value.y}};
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", Value.x);
            info.AddValue("y", Value.y);
        }
    }
}