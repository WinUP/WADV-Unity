using System.Collections.Generic;

namespace Core.VisualNovel.Script.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个条件分支表达式
    /// </summary>
    public class ConditionExpression : Expression {
        /// <summary>
        /// 分支内容列表
        /// </summary>
        public List<ConditionContentExpression> Contents { get; } = new List<ConditionContentExpression>();

        /// <inheritdoc />
        /// <summary>
        /// 创建一个条件分支表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public ConditionExpression(SourcePosition position) : base(position) {}
    }
}