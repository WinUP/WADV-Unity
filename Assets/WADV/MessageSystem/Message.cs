using System.Collections.Generic;
using WADV.Thread;

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
        /// 主线程占位符列表
        /// <para>如果某个消息监听器希望在消息传递给下一个节点后继续执行某些代码，可以通过主线程占位符通知消息循环何时可以结束</para>
        /// </summary>
        public List<MainThreadPlaceholder> Placeholders { get; } = new List<MainThreadPlaceholder>();
        
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

        /// <summary>
        /// 在此消息上新建一个主线程占位符
        /// </summary>
        /// <returns></returns>
        public MainThreadPlaceholder CreatePlaceholder() {
            var result = new MainThreadPlaceholder();
            Placeholders.Add(result);
            return result;
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
