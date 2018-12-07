namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 关键字和终止符
    /// </summary>
    public static class Keywords {
        /// <summary>
        /// 脚本语言
        /// </summary>
        public const string SyntaxLanguage = "lang";
        /// <summary>
        /// 选择指令
        /// </summary>
        public const string SyntaxIf = "if";
        /// <summary>
        /// 分支指令
        /// </summary>
        public const string SyntaxElseIf = "elseif";
        /// <summary>
        /// 否则指令
        /// </summary>
        public const string SyntaxElse = "else";
        /// <summary>
        /// 循环指令
        /// </summary>
        public const string SyntaxWhileLoop = "while";
        /// <summary>
        /// 函数指令
        /// </summary>
        public const string SyntaxFunction = "scene";
        /// <summary>
        /// 函数调用指令
        /// </summary>
        public const string SyntaxCall = "call";
        /// <summary>
        /// 返回指令
        /// </summary>
        public const string SyntaxReturn = "return";
        /// <summary>
        /// 导入指令
        /// </summary>
        public const string SyntaxImport = "import";
        /// <summary>
        /// 导出指令
        /// </summary>
        public const string SyntaxExport = "export";
        /// <summary>
        /// 所有操作符
        /// </summary>
        public static readonly string[] Separators = {"->", "+=", "-=", "*=", "/=", ">", "<", ">=", "<=", "[", "]", "!", "+", "-", "*", "/", "@", "@#", ";", "=", "==", "(", ")", " ", "\"", "\n"};

    }
}