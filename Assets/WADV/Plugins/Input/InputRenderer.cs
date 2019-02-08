using System.Threading.Tasks;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins.Input {
    /// <inheritdoc />
    /// <summary>
    /// 基础输入框组件
    /// </summary>
    public abstract class InputRenderer : MonoMessengerBehaviour {
        /// <inheritdoc />
        public override int Mask { get; } = InputPlugin.MessageIntegration.Mask;
        
        /// <inheritdoc />
        public override bool IsStandaloneMessage { get; } = false;
        
        /// <summary>
        /// 获取输入框文本
        /// </summary>
        public abstract string Text { get; }

        private bool _isShowing;

        public abstract void SetText(PluginExecuteContext context, InputPlugin.MessageIntegration.Content content);

        /// <summary>
        /// 显示输入框并等待输入
        /// </summary>
        /// <returns></returns>
        public abstract Task Show();

        /// <summary>
        /// 等待用户输入
        /// </summary>
        /// <returns></returns>
        public abstract Task Wait();

        /// <summary>
        /// 隐藏输入框
        /// </summary>
        /// <returns></returns>
        public abstract Task Hide();
        
        /// <inheritdoc />
        public override async Task<Message> Receive(Message message) {
            if (!_isShowing && message.HasTag(InputPlugin.MessageIntegration.CreateInput) && message is ContextMessage<InputPlugin.MessageIntegration.Content> inputMessage) {
                _isShowing = true;
                QuickCacheMessage(inputMessage);
                SetText(inputMessage.Context, inputMessage.Content);
                await Show();
                await Wait();
                await Hide();
                PopQuickCacheMessage();
                _isShowing = false;
                return Message<string>.Create(InputPlugin.MessageIntegration.Mask, null, Text);
            }
            if (_isShowing && message.HasTag(CoreConstant.LanguageChange) && message is Message<ChangeLanguageIntent> languageMessage) {
                inputMessage = PeekQuickCacheMessage<ContextMessage<InputPlugin.MessageIntegration.Content>>();
                if (inputMessage == null || inputMessage.Context.Runtime != languageMessage.Content.Runtime) return message;
                SetText(inputMessage.Context, inputMessage.Content);
            }
            return message;
        }
    }
}