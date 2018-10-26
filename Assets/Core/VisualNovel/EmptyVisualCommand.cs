using System.Collections;

namespace Assets.Core.VisualNovel {
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
