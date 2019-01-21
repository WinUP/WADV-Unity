using System.Collections.Generic;
using System.Linq;
using WADV.Thread;
using WADV.VisualNovel.Plugin;

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
        /// 创建一条空消息
        /// </summary>
        /// <param name="mask">消息掩码</param>
        /// <param name="tag">消息标记（如果有）</param>
        /// <returns></returns>
        public static Message Create(int mask, string tag = null) {
            return new Message(mask, tag);
        }

        /// <summary>
        /// 在此消息上新建一个主线程占位符
        /// </summary>
        /// <returns></returns>
        public MainThreadPlaceholder CreatePlaceholder() {
            var result = Dispatcher.CreatePlaceholder();
            Placeholders.Add(result);
            return result;
        }

        /// <summary>
        /// 确定消息是否具有指定标记（不传递任何标记时检查消息是否没有标记）
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(params string[] tag) {
            return tag.Length == 0 ? string.IsNullOrEmpty(Tag) : tag.Contains(Tag);
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
        /// 创建一条有值消息
        /// </summary>
        /// <param name="content">消息内容</param>
        public Message(T content) {
            Content = content;
        }

        /// <inheritdoc />
        /// <summary>
        /// 创建一条有值消息
        /// </summary>
        /// <param name="content">消息内容</param>
        /// <param name="mask">消息掩码</param>
        /// <param name="tag">消息标记（如果有）</param>
        public Message(T content, int mask, string tag = null) : base(mask, tag) {
            Content = content;
        }

        /// <summary>
        /// 创建一条有值消息
        /// </summary>
        /// <param name="content">消息内容</param>
        /// <param name="mask">消息掩码</param>
        /// <param name="tag">消息标记（如果有）</param>
        public static Message<T> Create(T content, int mask, string tag = null) {
            return new Message<T>(content, mask, tag);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// 表示一条带插件执行上下文的有值消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContextMessage<T> : Message<T> {
        /// <summary>
        /// 插件执行上下文
        /// </summary>
        public PluginExecuteContext Context { get; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一条带插件执行上下文的有值消息
        /// </summary>
        /// <param name="context">插件执行上下文</param>
        /// <param name="content">消息内容</param>
        public ContextMessage(PluginExecuteContext context, T content) : base(content) {
            Context = context;
        }

        /// <inheritdoc />
        /// <summary>
        /// 创建一条带插件执行上下文的有值消息
        /// </summary>
        /// <param name="context">插件执行上下文</param>
        /// <param name="content">消息内容</param>
        /// <param name="mask">消息掩码</param>
        /// <param name="tag">消息标记（如果有）</param>
        public ContextMessage(PluginExecuteContext context, T content, int mask, string tag = null) : base(content, mask, tag) {
            Context = context;
        }
        
        /// <summary>
        /// 创建一条带插件执行上下文的有值消息
        /// </summary>
        /// <param name="context">插件执行上下文</param>
        /// <param name="content">消息内容</param>
        /// <param name="mask">消息掩码</param>
        /// <param name="tag">消息标记（如果有）</param>
        public static ContextMessage<T> Create(PluginExecuteContext context, T content, int mask, string tag = null) {
            return new ContextMessage<T>(context, content, mask, tag);
        }
    }
}
