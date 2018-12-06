using System.Collections.Generic;

namespace Core.VisualNovel.Script.Compiler.Expressions {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个调用表达式
    /// </summary>
    public class CallExpression : Expression {
        /// <summary>
        /// 调用对象
        /// </summary>
        public Expression Target { get; set; }
        /// <summary>
        /// 参数列表
        /// </summary>
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();
        
        /// <inheritdoc />
        /// <summary>
        /// 创建一个调用表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        public CallExpression(CodePosition position) : base(position) {}
    }
}