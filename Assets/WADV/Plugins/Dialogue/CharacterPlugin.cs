using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Dialogue {
    /// <inheritdoc />
    /// <summary>
    /// <para>用于生成角色描述的插件</para>
    /// </summary>
    [StaticRegistrationInfo("Character")]
    public class CharacterPlugin : IVisualNovelPlugin {
        /// <inheritdoc />
        public void OnRegister() { }

        /// <inheritdoc />
        public void OnUnregister(bool isReplace) { }
        
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            var character = new CharacterValue();
            foreach (var (key, value) in context.StringParameters) {
                var stringValue = value as IStringConverter;
                switch (key.ConvertToString(context.Runtime.ActiveLanguage)) {
                    case "Name":
                        if (stringValue == null) {
                            Debug.LogWarning($"Skip parameter Name when creating Character: {value} is not string value");
                        } else {
                            character.name = stringValue;
                        }
                        break;
                    case "Avatar":
                        if (stringValue == null) {
                            Debug.LogWarning($"Skip parameter Avatar when creating Character: {value} is not string value");
                        } else {
                            character.avatar = stringValue;
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