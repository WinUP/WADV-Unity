using System;
using System.Runtime.Serialization;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Vector2 {
    [Serializable]
    public class Vector2Value : SerializableValue, ISerializable {

        public UnityEngine.Vector2 Value { get; set; }
        
        public override SerializableValue Duplicate() {
            throw new System.NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            throw new NotImplementedException();
        }
    }
}