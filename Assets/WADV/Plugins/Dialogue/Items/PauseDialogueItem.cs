namespace WADV.Plugins.Dialogue.Items {
    /// <summary>
    /// 用于冻结对话框一段时间的对话框内容
    /// </summary>
    public class PauseDialogueItem : IDialogueItem {
        /// <summary>
        /// 等待时间（为空时表示永久等待，需要用户手动取消，如点击屏幕）
        /// </summary>
        public float? Time { get; set; }
    }
}