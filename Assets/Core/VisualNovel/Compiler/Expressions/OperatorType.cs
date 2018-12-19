namespace Core.VisualNovel.Compiler.Expressions {
    /// <summary>
    /// 二元运算符类型
    /// </summary>
    public enum OperatorType {
        /// <summary>
        /// 取子元素
        /// </summary>
        PickChild,
        /// <summary>
        /// 加法
        /// </summary>
        Add,
        /// <summary>
        /// 自加
        /// </summary>
        AddBy,
        /// <summary>
        /// 减法
        /// </summary>
        Minus,
        /// <summary>
        /// 自减
        /// </summary>
        MinusBy,
        /// <summary>
        /// 乘法
        /// </summary>
        Multiply,
        /// <summary>
        /// 自乘
        /// </summary>
        MultiplyBy,
        /// <summary>
        /// 除法
        /// </summary>
        Divide,
        /// <summary>
        /// 自除
        /// </summary>
        DivideBy,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 小于
        /// </summary>
        LesserThan,
        /// <summary>
        /// 值比较
        /// </summary>
        EqualsTo,
        /// <summary>
        /// 不小于（大于等于）
        /// </summary>
        NotLessThan,
        /// <summary>
        /// 不大于（小于等于）
        /// </summary>
        NotGreaterThan,
        /// <summary>
        /// 真值比较
        /// </summary>
        LogicEqualsTo
    }
}