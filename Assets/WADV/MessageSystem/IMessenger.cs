using System.Threading.Tasks;

namespace WADV.MessageSystem {
    /// <summary>
    /// 表示一个消息监听器
    /// </summary>
    public interface IMessenger {
        /// <summary>
        /// 异步处理消息
        /// </summary>
        /// <param name="message">Message object</param>
        Task<Message> Receive(Message message);

        /// <summary>
        /// 可接受消息掩码
        /// <para>掩码与此监听器掩码位与结果不为0的被消息即被视为可接受</para>
        /// </summary>
        /// <returns></returns>
        int Mask { get; }
    }
}
