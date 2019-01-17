using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Extensions;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;

namespace WADV.Plugins.Dialogue {
    /// <inheritdoc />
    /// <summary>
    /// <para>用于生成角色描述的插件</para>
    /// </summary>
    [UsedImplicitly]
    public class CharacterPlugin : VisualNovelPlugin {
        public CharacterPlugin() : base("Character") { }
        
        public override Task<SerializableValue> Execute(PluginExecuteContext context) {
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