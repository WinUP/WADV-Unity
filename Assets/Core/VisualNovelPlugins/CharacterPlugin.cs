using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Extensions;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime;
using Core.VisualNovel.Runtime.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.VisualNovelPlugins {
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

        /// <inheritdoc cref="SerializableValue" />
        /// <summary>
        /// <para>表示一个角色内存值</para>
        /// <list type="bullet">
        ///     <listheader><description>互操作支持</description></listheader>
        ///     <item><description>字符串转换器</description></item>
        ///     <item><description>真值比较互操作器</description></item>
        /// </list>
        /// <list type="bullet">
        ///     <listheader><description>子元素/特性支持</description></listheader>
        ///     <item><description>Reverse</description></item>
        ///     <item><description>ToNumber</description></item>
        ///     <item><description>ToString</description></item>
        /// </list>
        /// </summary>
        [Serializable]
        public class CharacterValue : SerializableValue, IStringConverter, IEqualOperator, IPickChildOperator {
            /// <summary>
            /// 获取或设置角色名称
            /// </summary>
            [NotNull]
            public IStringConverter Name { get; set; }
            
            /// <summary>
            /// 获取或设置角色头像资源路径
            /// </summary>
            [NotNull]
            public IStringConverter Avatar { get; set; }
            
            /// <inheritdoc />
            public override SerializableValue Duplicate() {
                return new CharacterValue {Name = Name, Avatar = Avatar};
            }

            /// <inheritdoc />
            public string ConvertToString() {
                return Name.ConvertToString();
            }

            /// <inheritdoc />
            public string ConvertToString(string language) {
                return ConvertToString();
            }

            /// <inheritdoc />
            public bool EqualsWith(SerializableValue target) {
                return target is CharacterValue characterValue
                       && characterValue.Name.ConvertToString() == Name.ConvertToString()
                       && characterValue.Avatar.ConvertToString() == Avatar.ConvertToString();
            }

            /// <inheritdoc />
            public SerializableValue PickChild(SerializableValue name) {
                if (!(name is IStringConverter stringConverter))
                    throw new NotSupportedException($"Unable to get feature in character value with feature id {name}: only string feature name is accepted");
                var target = stringConverter.ConvertToString();
                switch (target) {
                    case "GetName":
                        return new StringValue {Value = !Value};
                    case "GetAvatar":
                        return new IntegerValue {Value = ConvertToInteger()};
                    case "ToString":
                        return new StringValue {Value = ConvertToString()};
                    default:
                        throw new NotSupportedException($"Unable to get feature in boolean value: unsupported feature {target}");
                }
            }
        }
    }
}