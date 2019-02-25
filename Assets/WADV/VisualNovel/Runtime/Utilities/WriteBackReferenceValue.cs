using System;
using JetBrains.Annotations;
using WADV.VisualNovel.Interoperation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="IStringConverter" />
    /// <summary>
    /// <para>表示一个可回写的间接引用内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>外部使用引用复制</description></item>
    ///     <item><description>被引用对象的复制方式取决于其本身</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>类型转换支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class WriteBackReferenceValue : ReferenceValue {
        private readonly Action<WriteBackReferenceValue> _writeBack;

        public override SerializableValue ReferenceTarget {
            get => base.ReferenceTarget;
            set {
                base.ReferenceTarget = value;
                _writeBack(this);
            }
        }

        public WriteBackReferenceValue([NotNull] Action<WriteBackReferenceValue> writeBackFunction) {
            _writeBack = writeBackFunction;
        }

        public WriteBackReferenceValue(SerializableValue referenceTarget, [NotNull] Action<WriteBackReferenceValue> writeBackFunction) : base(referenceTarget) {
            _writeBack = writeBackFunction;
        }

        public override SerializableValue Duplicate() {
            return new WriteBackReferenceValue(ReferenceTarget.Duplicate(), _writeBack) {IsConstant = IsConstant};
        }
    }
}