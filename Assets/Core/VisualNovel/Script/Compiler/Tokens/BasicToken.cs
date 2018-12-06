namespace Core.VisualNovel.Script.Compiler.Tokens {
    /// <summary>
    /// 表示一个基础标记
    /// </summary>
    public class BasicToken {
        /// <summary>
        /// 标记类型
        /// </summary>
        public TokenType Type { get; }
        /// <summary>
        /// 该标记在源代码中的对应位置
        /// </summary>
        public CodePosition Position { get; }

        /// <summary>
        /// 创建一个基础标记
        /// </summary>
        /// <param name="type">标记类型</param>
        /// <param name="position">该标记在源代码中的对应位置</param>
        public BasicToken(TokenType type, CodePosition position) {
            Type = type;
            Position = position;
        }
    }
}
