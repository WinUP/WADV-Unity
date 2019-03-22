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

        public const string UpdateInformation = "UPDATE_INFORMATION";

        public const string GetCanvasSize = "GET_CANVAS_SIZE";

        public const string GetBindShader = "GET_BIND_SHADER";
        
        public class ShowImageContent {
            [CanBeNull]
            public SingleGraphicEffect Effect { get; set; }
            
            public List<ImageProperties> Images { get; set; } = new List<ImageProperties>();
        }
    }
}