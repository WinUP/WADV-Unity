using System.Threading.Tasks;

namespace WADV.MessageSystem {
    /// <inheritdoc />
    /// <summary>
    /// 空消息接收器
    /// </summary>
    public class EmptyMessenger : IMessenger {
        public int Mask { get; set; }

        public EmptyMessenger(int mask = int.MaxValue) {
            Mask = mask;
        }
        
        public Task<Message> Receive(Message message) {
            return Task.FromResult(message);
        }
    }
}