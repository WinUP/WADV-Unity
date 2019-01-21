using WADV.MessageSystem;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Dialogue {
    public partial class DialoguePlugin {
        public static class MessageIntegration {
            /// <summary>
            /// 插件使用的消息掩码
            /// </summary>
            public const int Mask = CoreConstant.Mask;
        
            /// <summary>
            /// 表示新建对话的消息标记
            /// </summary>
            public const string NewDialogue = "NEW_DIALOGUE";

            /// <summary>
            /// 表示显示对话框的消息标记
            /// </summary>
            public const string ShowDialogueBox = "SHOW_DIALOGUE_TEXT";
        
            /// <summary>
            /// 表示隐藏对话框的消息标记
            /// </summary>
            public const string HideDialogueBox = "HIDE_DIALOGUE_TEXT";

            /// <summary>
            /// 表示对话框内容显示等待完成的消息标记
            /// </summary>
            public const string FinishContentWaiting = "FINISH_CONTENT_WAITING";
            
            /// <summary>
            /// 对话框消息内容
            /// </summary>
            public class Content {
                /// <summary>
                /// 原始对话角色
                /// </summary>
                public SerializableValue Character { get; set; }
        
                /// <summary>
                /// 原始对话内容
                /// </summary>
                public IStringConverter Text { get; set; }
            }
        }
    }
}