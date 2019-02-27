using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Plugins.Vector;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Provider;

namespace WADV.Plugins.Image {
    [Serializable]
    public class ImageValue : SerializableValue, ISerializable {
        public uint color;

        public Rect2Value uv;

        public string source;

        public Texture2D texture;

        public async Task ReadTexture(string newSource = null) {
            if (!string.IsNullOrEmpty(newSource)) {
                source = newSource;
            }
            texture = await ResourceManager.Load<Texture2D>(source);
        }
        
        public override SerializableValue Duplicate() {
            throw new System.NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            throw new NotImplementedException();
        }
    }
}