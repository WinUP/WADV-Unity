using System;
using JetBrains.Annotations;
using WADV.Plugins.Unity;

namespace WADV.Plugins.Image.Utilities {
    /// <summary>
    /// 图片信息
    /// </summary>
    [Serializable]
    public class ImageProperties {
        /// <summary>
        /// 显示名称
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// 图片内容
        /// </summary>
        [NotNull]
        public ImageValue Content { get; }

        /// <summary>
        /// 图片的Transform属性集
        /// </summary>
        [CanBeNull]
        public TransformValue Transform { get; }

        public ImageProperties([NotNull] string name, [NotNull] ImageValue image, [CanBeNull] TransformValue transform) {
            Name = name;
            Content = image;
            Transform = transform;
        }
    }
}