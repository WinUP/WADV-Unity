using System.Collections.Generic;

namespace Core.VisualNovel.Script.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个函数调用表达式
    /// </summary>
    public class FunctionCallExpression : Expression {
        /// <summary>
        /// 函数名
        /// </summary>
        public Expression Target { get; set; }
        /// <summary>
        /// 函数参数列表
        /// </summary>
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();
        
        /// <summary>
        /// 创建一个函数调用表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public FunctionCallExpression(SourcePosition position) : base(position) {}
    }
}