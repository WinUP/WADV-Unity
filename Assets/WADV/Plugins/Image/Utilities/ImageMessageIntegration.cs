using JetBrains.Annotations;
using WADV.Plugins.Effect;

namespace WADV.Plugins.Image.Utilities {
    public static class ImageMessageIntegration {
        /// <summary>
        /// 插件使用的消息掩码
        /// </summary>
        public const int Mask = 0B10;

        public const string ShowImage = "SHOW_IMAGE";

        public const string HideImage = "HIDE_IMAGE";

        public const string UpdateInformation = "UPDATE_INFORMATION";

        public const string GetCanvasSize = "GET_CANVAS_SIZE";

        public const string GetBindShader = "GET_BIND_SHADER";

        public const string PlayEffect = "PLAY_EFFECT";

        public const string StopEffect = "STOP_EFFECT";
        
        public struct ShowImageContent {
            [CanBeNull] public SingleGraphicEffect Effect;

            public ImageDisplayInformation[] Images;
        }

        public struct HideImageContent {
            [CanBeNull] public SingleGraphicEffect Effect;

            public string[] Names;
        }

        public struct PlayEffectContent {
            public GraphicEffect Effect;
            
            public string[] Names;
        }
    }
}