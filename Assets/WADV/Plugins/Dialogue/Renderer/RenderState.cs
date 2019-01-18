namespace WADV.Plugins.Dialogue.Renderer {
    /// <summary>
    /// 对话框渲染状态
    /// </summary>
    public enum RenderState {
        /// <summary>
        /// 正在渲染
        /// </summary>
        Rendering,
        /// <summary>
        /// 定时等待或等待用户操作
        /// </summary>
        Waiting,
        /// <summary>
        /// 空闲
        /// </summary>
        Idle
    }
}