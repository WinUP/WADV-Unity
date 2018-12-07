namespace Core.VisualNovel.Script.Compiler.Tokens {
    /// <summary>
    /// 标记类型
    /// </summary>
    public enum TokenType {
        /// <summary>
        /// 快速对话角色
        /// </summary>
        DialogueSpeaker,
        /// <summary>
        /// 快速对话内容
        /// </summary>
        DialogueContent,
        /// <summary>
        /// 有效换行符
        /// </summary>
        LineBreak,
        /// <summary>
        /// 字符串字面量
        /// </summary>
        String,
        /// <summary>
        /// 数字字面量
        /// </summary>
        Number,
        /// <summary>
        /// 作用域起点（缩进）
        /// </summary>
        CreateScope,
        /// <summary>
        /// 作用域终点
        /// </summary>
        LeaveScope,
        /// <summary>
        /// 插件调用开始
        /// </summary>
        PluginCallStart,
        /// <summary>
        /// 插件调用结束
        /// </summary>
        PluginCallEnd,
        /// <summary>
        /// 变量引用
        /// </summary>
        Variable,
        /// <summary>
        /// 常量引用
        /// </summary>
        Constant,
        /// <summary>
        /// 左小括号
        /// </summary>
        LeftBracket,
        /// <summary>
        /// 右小括号
        /// </summary>
        RightBracket,
        /// <summary>
        /// 取子元素/子插件
        /// </summary>
        PickChild,
        /// <summary>
        /// 自减运算符
        /// </summary>
        MinusEqual,
        /// <summary>
        /// 减法运算符
        /// </summary>
        Minus,
        /// <summary>
        /// 自加运算符
        /// </summary>
        AddEqual,
        /// <summary>
        /// 加法运算符
        /// </summary>
        Add,
        /// <summary>
        /// 自乘运算符
        /// </summary>
        MultiplyEqual,
        /// <summary>
        /// 乘法运算符
        /// </summary>
        Multiply,
        /// <summary>
        /// 自除运算符
        /// </summary>
        DivideEqual,
        /// <summary>
        /// 除法运算符
        /// </summary>
        Divide,
        /// <summary>
        /// 大于等于运算符
        /// </summary>
        GreaterEqual,
        /// <summary>
        /// 等于运算符
        /// </summary>
        Equal,
        /// <summary>
        /// 大于运算符
        /// </summary>
        Greater,
        /// <summary>
        /// 小于等于运算符
        /// </summary>
        LesserEqual,
        /// <summary>
        /// 小于运算符
        /// </summary>
        Lesser,
        /// <summary>
        /// 逻辑取反运算符
        /// </summary>
        LogicNot,
        /// <summary>
        /// 真值比较运算符
        /// </summary>
        LogicEqual,
        /// <summary>
        /// 函数声明
        /// </summary>
        Function,
        /// <summary>
        /// 条件分支
        /// </summary>
        If,
        /// <summary>
        /// 额外条件分支
        /// </summary>
        ElseIf,
        /// <summary>
        /// 默认条件分支
        /// </summary>
        Else,
        /// <summary>
        /// 循环
        /// </summary>
        Loop,
        /// <summary>
        /// 脚本语言切换
        /// </summary>
        Language,
        /// <summary>
        /// 返回
        /// </summary>
        Return,
        /// <summary>
        /// 函数调用
        /// </summary>
        FunctionCall,
        /// <summary>
        /// 导入脚本
        /// </summary>
        Import,
        /// <summary>
        /// 导出脚本
        /// </summary>
        Export
    }
}
