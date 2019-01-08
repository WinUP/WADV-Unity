namespace WADV.MessageSystem {
    /// <summary>
    /// 表示一条空消息
    /// </summary>
    public class Message {

        /// <summary>
        /// 消息标记
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 消息掩码
        /// </summary>
        public int Mask { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// 表示一条有值消息
    /// </summary>
    public class Message<T> : Message {
        /// <summary>
        /// 消息内容
        /// </summary>
        public T Content { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 新建一条有值消息
        /// </summary>
        /// <param name="content">Message's content</param>
        public Message(T content) {
            Content = content;
        }
    }
}
