using System.Collections.Generic;
using JetBrains.Annotations;
using WADV.MessageSystem;
using WADV.Plugins.Image.Effects;

namespace WADV.Plugins.Image.Utilities {
    public static class ImageMessageIntegration {
        /// <summary>
        /// 插件使用的消息掩码
        /// </summary>
        public const int Mask = CoreConstant.Mask;

        public const string ShowImage = "SHOW_IMAGE";

        public const string GetArea = "GET_AREA";

        public class GetAreaContent {
            public List<ImageInformation> Images { get; set; } = new List<ImageInformation>();
        }
        
        public class ShowImageContent {
            public ShowPlugin.BindMode Mode { get; set; } = ShowPlugin.BindMode.None;
                
            [CanBeNull]
            public SingleGraphicEffect Effect { get; set; }
                
            public List<ImageInformation> Images { get; set; } = new List<ImageInformation>();
        }
    }
}