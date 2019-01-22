using System.Threading.Tasks;
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

        /// <summary>
        /// 显示输入框并等待输入
        /// </summary>
        /// <param name="context">插件执行上下文</param>
        /// <param name="content">输入框参数</param>
        /// <returns></returns>
        public abstract Task Show(PluginExecuteContext context, InputPlugin.MessageIntegration.Content content);

        /// <summary>
        /// 隐藏输入框
        /// </summary>
        /// <returns></returns>
        public abstract Task Hide();
        
        /// <inheritdoc />
        public override async Task<Message> Receive(Message message) {
            if (_isShowing || !message.HasTag(InputPlugin.MessageIntegration.CreateInput)
                           || !(message is ContextMessage<InputPlugin.MessageIntegration.Content> inputMessage)) return message;
            _isShowing = true;
            await Show(inputMessage.Context, inputMessage.Content);
            await Hide();
            _isShowing = false;
            return Message<string>.Create(Text, InputPlugin.MessageIntegration.Mask, InputPlugin.MessageIntegration.InputText);
        }
    }
}