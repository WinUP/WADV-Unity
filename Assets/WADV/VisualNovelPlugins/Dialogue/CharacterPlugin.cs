using System.Collections.Generic;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime;
using JetBrains.Annotations;
using UnityEngine;

namespace WADV.VisualNovelPlugins.Dialogue {
    /// <inheritdoc />
    /// <summary>
    /// <para>用于生成角色描述的插件</para>
    /// </summary>
    [UsedImplicitly]
    public class CharacterPlugin : VisualNovelPlugin {
        public CharacterPlugin() : base("Character") { }
        
        public override Task<SerializableValue> Execute(ScriptRuntime context, IDictionary<SerializableValue, SerializableValue> parameters) {
            var character = new CharacterValue();
            foreach (var (key, value) in parameters) {
                string parameterName;
                if (key is IStringConverter stringKey) {
                    parameterName = stringKey.ConvertToString(context.ActiveLanguage);
                } else {
                    continue;
                }
                var stringValue = value as IStringConverter;
                switch (parameterName) {
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