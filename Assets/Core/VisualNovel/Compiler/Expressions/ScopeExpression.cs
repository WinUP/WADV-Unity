using System.Collections.Generic;

namespace Core.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个作用域表达式
    /// </summary>
    public class ScopeExpression : Expression {
        /// <summary>
        /// 作用域内内容
        /// </summary>
        public List<Expression> Content { get; set; } = new List<Expression>();
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个作用域表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public ScopeExpression(SourcePosition position) : base(position) {}
    }
}