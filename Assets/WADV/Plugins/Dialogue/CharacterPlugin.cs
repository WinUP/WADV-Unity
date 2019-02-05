using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins.Dialogue {
    /// <inheritdoc />
    /// <summary>
    /// <para>用于生成角色描述的插件</para>
    /// </summary>
    [UseStaticRegistration("Character")]
    public class CharacterPlugin : IVisualNovelPlugin {
        /// <inheritdoc />
        public bool OnRegister() => true;

        /// <inheritdoc />
        public bool OnUnregister(bool isReplace) => true;
        
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var character = new CharacterValue();
            foreach (var (key, value) in context.StringParameters) {
                var stringValue = value as IStringConverter;
                switch (key.ConvertToString(context.Runtime.ActiveLanguage)) {
                    case "Name":
                        if (stringValue == null) {
                            Debug.LogWarning($"Skip parameter Name when creating Character: {value} is not string value");
                        } else {
                            character.Name = stringValue;
                        }
                        break;
                    case "Avatar":
                        if (stringValue == null) {
                            Debug.LogWarning($"Skip parameter Avatar when creating Character: {value} is not string value");
                        } else {
                            character.Avatar = stringValue;
                        }
                        break;
                    default:
                        continue;
                }
            }
            return Task.FromResult<SerializableValue>(character);
        }
    }
}