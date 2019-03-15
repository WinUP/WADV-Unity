using System;
using UnityEngine;
using WADV.Plugins.Image.Utilities;

namespace WADV.Plugins.Image {
    public partial class ShowPlugin {

        [Serializable]
        private class ShowingInformation {
            public int Layer { get; }
            
            public Rect DisplayArea { get; set; }
            
            public ShowingInformation(int layer) {
                Layer = layer;
            }
            
            public ImageProperties Image { get; set; }
        }
    }
}