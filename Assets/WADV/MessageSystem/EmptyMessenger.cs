using System.Threading.Tasks;

namespace WADV.MessageSystem {
    /// <inheritdoc />
    /// <summary>
    /// 空消息接收器
    /// </summary>
    public class EmptyMessenger : IMessenger {
        /// <inheritdoc />
        public int Mask { get; }
        /// <inheritdoc />
        public bool IsStandaloneMessage { get; } = false;

        /// <summary>
        /// 创建一个空消息接收器
        /// </summary>
        /// <param name="mask">可处理的消息的掩码（默认为2^31 - 1）</param>
        public EmptyMessenger(int mask = int.MaxValue) {
            Mask = mask;
        }
        
        /// <inheritdoc />
        public Task<Message> Receive(Message message) {
            return Task.FromResult(message);
        }
    }
}