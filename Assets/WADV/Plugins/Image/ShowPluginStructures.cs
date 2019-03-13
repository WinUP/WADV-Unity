using System;

namespace WADV.Plugins.Image {
    public partial class ShowPlugin {
        public enum BindMode {
            Canvas,
            Minimal,
            None
        }

        [Serializable]
        private class ShowingImage {
            public int Layer { get; set; }
            
            public ImageValue Image { get; set; }
        }
    }
}