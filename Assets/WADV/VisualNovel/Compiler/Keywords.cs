namespace WADV.VisualNovel.Compiler {
    /// <summary>
    /// 关键字和终止符
    /// </summary>
    public static class Keywords {
        /// <summary>
        /// 脚本语言
        /// </summary>
        public const string Language = "lang";
        /// <summary>
        /// 选择指令
        /// </summary>
        public const string If = "if";
        /// <summary>
        /// 分支指令
        /// </summary>
        public const string ElseIf = "elseif";
        /// <summary>
        /// 否则指令
        /// </summary>
        public const string Else = "else";
        /// <summary>
        /// 循环指令
        /// </summary>
        public const string WhileLoop = "while";
        /// <summary>
        /// 函数指令
        /// </summary>
        public const string Function = "scene";
        /// <summary>
        /// 函数调用指令
        /// </summary>
        public const string Call = "call";
        /// <summary>
        /// 返回指令
        /// </summary>
        public const string Return = "return";
        /// <summary>
        /// 导入指令
        /// </summary>
        public const string Import = "import";
        /// <summary>
        /// 导出指令
        /// </summary>
        public const string Export = "export";
        /// <summary>
        /// 所有操作符
        /// </summary>
        public static readonly string[] Separators = {"->", "+=", "-=", "*=", "/=", ">", "<", ">=", "<=", "[", "]", "!", "+", "-", "*", "/", "@", "@#", ";", "=", "==", "!=", "(", ")", " ", "\"", "\n", "'"};
    }
}