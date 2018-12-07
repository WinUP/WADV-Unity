namespace Core.VisualNovel.Script.Compiler.Expressions {
    /// <summary>
    /// 表示一个基础表达式
    /// <para>基础表达式是抽象语法树的所有节点的基础，但其自身不能作为节点使用，必须由其他继承类表示</para>
    /// </summary>
    public abstract class Expression {
        /// <summary>
        /// 该表达式在源代码中的对应位置
        /// </summary>
        public SourcePosition Position { get; }

        /// <summary>
        /// 创建一个表达式
        /// </summary>
        /// <param name="position">该表达式在源代码中的对应位置</param>
        protected Expression(SourcePosition position) {
            Position = position;
        }
    }
}