using UnityEngine;

namespace WADV.Thread {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个Unity主循环占位符
    /// </summary>
    public class MainThreadPlaceholder : CustomYieldInstruction {
        /// <inheritdoc />
        public override bool keepWaiting => !_finished;

        private bool _finished;

        /// <summary>
        /// 释放该占位符
        /// </summary>
        public void Complete() {
            _finished = true;
        }
    }
}