using JetBrains.Annotations;
using WADV.MessageSystem;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Input {
    public partial class InputPlugin {
        public static class MessageIntegration {
            /// <summary>
            /// 插件使用的消息掩码
            /// </summary>
            public const int Mask = 0B100;
        
            /// <summary>
            /// 表示新建输入框的消息标记
            /// </summary>
            public const string CreateInput = "CREATE_INPUT";
            
            /// <summary>
            /// 输入框消息内容
            /// </summary>
            public struct Content {
                /// <summary>
                /// 标题
                /// </summary>
                [CanBeNull]
                public IStringConverter Title { get; set; }

                /// <summary>
                /// 默认值
                /// </summary>
                [CanBeNull]
                public IStringConverter Default { get; set; }
        
                /// <summary>
                /// 确定按钮文本
                /// </summary>
                [CanBeNull]
                public IStringConverter ButtonText { get; set; }
            }
        }
    }
}