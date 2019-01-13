using System.Threading.Tasks;

namespace WADV.MessageSystem {
    /// <summary>
    /// 表示一个消息监听器
    /// </summary>
    public interface IMessenger {
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="message">Message object</param>
        Task<Message> Receive(Message message);

        /// <summary>
        /// 可接受消息掩码
        /// <para>掩码与此监听器掩码位与结果不为0的被消息即被视为可接受</para>
        /// </summary>
        /// <returns></returns>
        int Mask { get; }
        
        /// <summary>
        /// 是否为将此接收器分配至独立执行队列，这将使得消息循环将此消息监听器交由Unity协程管理并直接运行下一个消息监听器
        /// </summary>
        bool IsStandaloneMessage { get; }
    }
}
