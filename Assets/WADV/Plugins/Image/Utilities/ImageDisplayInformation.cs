using System;
using UnityEngine;

namespace WADV.Plugins.Image.Utilities {
    [Serializable]
    public struct ImageDisplayInformation {
        public int Layer { get; set; }
        
        public Rect DisplayArea { get; set; }
        
        public ImageStatus Status { get; set; }

        public ImageDisplayInformation To(ImageStatus status) {
            return new ImageDisplayInformation {
                Layer = Layer,
                DisplayArea = DisplayArea,
                Status = status
            };
        }
    }
}