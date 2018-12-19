#pragma warning disable 1998

using System.Threading.Tasks;

namespace Core.MessageSystem {
    /// <inheritdoc />
    /// <summary>
    /// 空消息接收器
    /// </summary>
    public class EmptyMessenger : IMessenger {
        public int Mask { get; set; }

        public EmptyMessenger(int mask = int.MaxValue) {
            Mask = mask;
        }
        
        public async Task<Message> Receive(Message message) {
            return message;
        }
    }
}