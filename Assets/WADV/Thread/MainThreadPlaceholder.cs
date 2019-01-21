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
        public virtual void Complete() {
            _finished = true;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// 表示一个带返回值的Unity主循环占位符
    /// </summary>
    public class MainThreadPlaceholder<T> : MainThreadPlaceholder {
        /// <summary>
        /// 返回值
        /// </summary>
        public T Value { get; private set; }

        /// <inheritdoc />
        public override void Complete() {
            Value = default(T);
            base.Complete();
        }

        /// <summary>
        /// 释放该占位符
        /// </summary>
        /// <param name="value">返回值</param>
        public void Complete(T value) {
            Value = value;
            base.Complete();
        }
    }
}