using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Converter {
    /// <inheritdoc />
    /// <summary>
    /// <para>用于将目标值解析为32位整数的插件</para>
    /// <list type="bullet">
    ///     <listheader><description>可选无值参数</description></listheader>
    ///     <item><description>要转换的数据</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>可选有值参数</description></listheader>
    ///     <item><description>Value: 要转换的数据</description></item>
    /// </list>
    /// </summary>
    /// <remarks>插件仅会解析第一个参数，如果没有参数传入则返回32位整数0。第一个参数若包含无法被转换的结构必定出现错误。</remarks>
    [StaticRegistrationInfo("Integer")]
    [UsedImplicitly]
    public class IntegerPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            foreach (var (key, value) in context.Parameters) {
                return Task.FromResult<SerializableValue>(new IntegerValue {
                    value = IntegerValue.TryParse(
                        key is IStringConverter stringKey && stringKey.ConvertToString(context.Language) == "Value" ? value : key
                    )
                });
            }
            return Task.FromResult<SerializableValue>(new IntegerValue {value = 0});
        }

        public void OnRegister() { } 

        public void OnUnregister(bool isReplace) { }
    }
}