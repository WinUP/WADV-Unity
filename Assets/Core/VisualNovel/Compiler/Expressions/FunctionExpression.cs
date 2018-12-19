using System.Collections.Generic;

namespace Core.VisualNovel.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个函数表达式
    /// </summary>
    public class FunctionExpression : Expression {
        /// <summary>
        /// 函数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 函数参数及默认值
        /// </summary>
        public List<StaticNameParameterExpression> Parameters { get; } = new List<StaticNameParameterExpression>();
        /// <summary>
        /// 函数内容
        /// </summary>
        public ScopeExpression Body { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个函数表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public FunctionExpression(SourcePosition position) : base(position) {}
    }
}