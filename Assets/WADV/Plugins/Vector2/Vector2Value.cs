using System;
using System.Runtime.Serialization;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Vector2 {
    [Serializable]
    public class Vector2Value : SerializableValue, ISerializable {
        public UnityEngine.Vector2 Value { get; set; }
        
        public Vector2Value() { }
        
        protected Vector2Value(SerializationInfo info, StreamingContext context) {
            Value = new UnityEngine.Vector2(info.GetSingle("x"), info.GetSingle("y"));
        }
        
        public override SerializableValue Duplicate() {
            return new Vector2Value {Value = new UnityEngine.Vector2(Value.x, Value.y)};
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", Value.x);
            info.AddValue("y", Value.y);
        }
    }
}