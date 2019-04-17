using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
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

        public void ApplyTo(GameObject target, Transform layoutRoot = null) {
            var targetTransform = target.GetComponent<RectTransform>() ?? target.AddComponent<RectTransform>();
            if (layoutRoot != null && targetTransform.parent != layoutRoot) {
                targetTransform.SetParent(layoutRoot);
            }
            if (Content.Texture.texture != null) {
                targetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Content.Texture.texture.height);
                targetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Content.Texture.texture.width);
                targetTransform.localScale = Vector3.one;
            }
            var position = targetTransform.localPosition;
            targetTransform.localPosition = new Vector3(position.x, position.y, 0);
            Transform?.ApplyTo(targetTransform);
            var targetImage = target.GetComponent<RawImage>() ?? target.AddComponent<RawImage>();
            targetImage.texture = Content.Texture.texture;
            targetImage.uvRect = Content.Uv.value;
            targetImage.color = Content.Color.value;
        }
    }
}