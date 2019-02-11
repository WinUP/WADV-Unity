using System;
using System.Runtime.Serialization;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Vector {
    [Serializable]
    public class Vector3Value : SerializableValue, ISerializable {
        public UnityEngine.Vector3 Value { get; set; }
        
        public Vector3Value() { }
        
        protected Vector3Value(SerializationInfo info, StreamingContext context) {
            Value = new UnityEngine.Vector3(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"));
        }
        
        public override SerializableValue Duplicate() {
            return new Vector2Value {Value = new UnityEngine.Vector3(Value.x, Value.y, Value.z)};
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", Value.x);
            info.AddValue("y", Value.y);
            info.AddValue("z", Value.z);
        }
    }
}