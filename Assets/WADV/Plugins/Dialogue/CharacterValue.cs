using System;
using WADV.Translation;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Dialogue {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个角色内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    ///     <item><description>真值比较互操作器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class CharacterValue : SerializableValue, IStringConverter, IEqualOperator {
        /// <summary>
        /// 获取或设置角色名称
        /// </summary>
        public IStringConverter name;

        /// <summary>
        /// 获取或设置角色头像资源路径
        /// </summary>
        public IStringConverter avatar;
            
        /// <inheritdoc />
        public override SerializableValue Clone() {
            return new CharacterValue {name = name, avatar = avatar};
        }

        /// <inheritdoc />
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return name.ConvertToString(language);
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is CharacterValue characterValue
                   && characterValue.name.ConvertToString(language) == name.ConvertToString(language)
                   && characterValue.avatar.ConvertToString(language) == avatar.ConvertToString(language);
        }
    }
}