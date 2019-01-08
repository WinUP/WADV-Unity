using System;
using WADV.VisualNovel.Interoperation;
using JetBrains.Annotations;

namespace WADV.VisualNovelPlugins.Dialogue {
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
    }
}