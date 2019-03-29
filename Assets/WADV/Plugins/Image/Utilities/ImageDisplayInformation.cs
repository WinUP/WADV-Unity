using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Plugins.Unity;

namespace WADV.Plugins.Image.Utilities {
    [Serializable]
    public struct ImageDisplayInformation : ISerializable {
        public readonly string Name;
        
        public readonly ImageValue Content;
        
        [CanBeNull]
        public readonly TransformValue Transform;
        
        public int layer;

        public Matrix4x4 displayMatrix;

        public ImageStatus status;
        
        public ImageDisplayInformation([NotNull] string name, [NotNull] ImageValue image, [CanBeNull] TransformValue transform) {
            Name = name;
            Content = image;
            Transform = transform;
            layer = 0;
            displayMatrix = Matrix4x4.identity;
            status = ImageStatus.Unavailable;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            throw new NotImplementedException();
        }
    }
}