using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
 * 简化的工作流系统
 *
 * 单入单出
 * 多出选一
 * 支持协程
 * 无参数列表
 */

namespace Assets.Core.VisualNovel {
    /// <summary>
    /// 基础AVG指令
    /// </summary>
    public abstract class VisualCommand {
        /// <summary>
        /// 指令ID
        /// </summary>
        public string ID { get; }
        /// <summary>
        /// 所有可选的下一个指令
        /// </summary>
        public List<VisualCommand> NextCommand { get; }

        /// <summary>
        /// 创建一个AVG指令
        /// </summary>
        /// <param name="id">指令ID</param>
        public VisualCommand(string id) {
            ID = id;
            NextCommand = new List<VisualCommand>();
        }

        /// <summary>
        /// 运行指令
        /// </summary>
        public abstract IEnumerator Run();

        /// <summary>
        /// 获取下一个指令（为空时终止指令网络）
        /// </summary>
        /// <returns></returns>
        public virtual VisualCommand Next() {
            return NextCommand.First();
        }
    }
}
