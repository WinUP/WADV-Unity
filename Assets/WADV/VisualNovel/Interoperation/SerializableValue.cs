using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Intents;

namespace WADV.VisualNovel.Interoperation {
    /// <summary>
    /// 表示一个可序列化内存值
    /// </summary>
    [Serializable]
    public abstract class SerializableValue {
        protected SerializableValue() {
            if (!GetType().IsSerializable) throw new NotSupportedException($"Missing Serializable attribute: {GetType().FullName} is not serializable value");
        }
        
        /// <summary>
        /// 获得该值的一个副本
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public abstract SerializableValue Clone();

        /// <summary>
        /// 转储该值前的准备工作
        /// </summary>
        /// <param name="tasks">转储管理器等待任务列表</param>
        /// <returns></returns>
        public virtual Task OnDump(DumpRuntimeIntent.TaskLists tasks) {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 从转储文件读取该值后的额外工作
        /// </summary>
        /// <param name="tasks">转储管理器等待任务列表</param>
        /// <returns></returns>
        public virtual Task OnRead(DumpRuntimeIntent.TaskLists tasks) {
            return Task.CompletedTask;
        }
    }
}