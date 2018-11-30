using System.Collections;
using Assets.Core.VisualNovel;

namespace Core.VisualNovel {
    /// <inheritdoc />
    /// <summary>
    /// 空指令节点
    /// </summary>
    public class EmptyVisualCommand : VisualCommand {
        public EmptyVisualCommand(string id) : base(id) { }

        public override IEnumerator Run() {
            return null;
        }
    }
}
