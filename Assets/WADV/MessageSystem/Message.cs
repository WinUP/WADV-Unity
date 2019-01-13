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
        
        /// <summary>
        /// 创建一条空消息，你之后必须手动为此消息分配掩码（必须）和标记（如果有）
        /// </summary>
        public Message() { }

        /// <summary>
        /// 创建一条空消息
        /// </summary>
        /// <param name="mask">消息掩码</param>
        /// <param name="tag">消息标记（如果有）</param>
        public Message(int mask, string tag = null) {
            Mask = mask;
            Tag = tag;
        }
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
        /// <param name="content">消息内容</param>
        public Message(T content) {
            Content = content;
        }

        /// <inheritdoc />
        /// <summary>
        /// 新建一条有值消息
        /// </summary>
        /// <param name="content">消息内容</param>
        /// <param name="mask">消息掩码</param>
        /// <param name="tag">消息标记（如果有）</param>
        public Message(T content, int mask, string tag = null) : base(mask, tag) {
            Content = content;
        }
    }
}
