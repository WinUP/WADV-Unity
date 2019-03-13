using JetBrains.Annotations;
using WADV.Plugins.Unity;

namespace WADV.Plugins.Image.Utilities {
    public class ImageInformation {
        [CanBeNull]
        public string Name { get; set; }

        [CanBeNull]
        public ImageValue Image { get; set; }

        [CanBeNull]
        public TransformValue Transform { get; set; }
    }
}