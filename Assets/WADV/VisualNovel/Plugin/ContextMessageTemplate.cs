namespace WADV.VisualNovel.Plugin {
    /// <summary>
    /// 带执行上下文的插件消息模板，用于在插件广播消息时提供对当前执行上下文的引用
    /// </summary>
    public class ContextMessageTemplate {
        /// <summary>
        /// 获取插件的执行上下文
        /// </summary>
        public PluginExecuteContext Context { get; }

        /// <summary>
        /// 创建一个插件消息模板
        /// </summary>
        /// <param name="context">执行上下文</param>
        public ContextMessageTemplate(PluginExecuteContext context) {
            Context = context;
        }
    }
}