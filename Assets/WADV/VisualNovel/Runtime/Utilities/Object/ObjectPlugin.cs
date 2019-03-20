using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;

namespace WADV.VisualNovel.Runtime.Utilities.Object {
    /// <inheritdoc />
    /// <summary>
    /// 为VNS提供对象支持（一定程度上可充当异构数组使用）
    /// </summary>
    [StaticRegistrationInfo("Object")]
    [UsedImplicitly]
    public class ObjectPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var result = new ObjectValue();
            foreach (var (key, value) in context.Parameters) {
                result.Add(key, value, context.Language);
            }
            return Task.FromResult<SerializableValue>(result);
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}