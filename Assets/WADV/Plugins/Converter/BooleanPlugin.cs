using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Converter {
    /// <inheritdoc />
    /// <summary>
    /// <para>用于将目标值解析为布尔值的插件</para>
    /// <list type="bullet">
    ///     <listheader><description>必选匿名参数</description></listheader>
    ///     <item><description>要转换的数据</description></item>
    /// </list>
    /// </summary>
    /// <remarks>插件仅会解析第一个参数，如果没有参数传入则返回false。第一个参数若包含无法被转换的结构必定出现错误。</remarks>
    [StaticRegistrationInfo("Boolean")]
    [UsedImplicitly]
    public class BooleanPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            foreach (var (key, _) in context.Parameters) {
                return Task.FromResult<SerializableValue>(new BooleanValue {
                    value = BooleanValue.TryParse(key, context.Language)
                });
            }
            return Task.FromResult<SerializableValue>(new BooleanValue {value = false});
        }

        public void OnRegister() { } 

        public void OnUnregister(bool isReplace) { }
    }
}