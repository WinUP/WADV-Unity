namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 表示一个源代码坐标
    /// </summary>
    public struct CodePosition {
        /// <summary>
        /// 行号（从0开始）
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// 列号（从0开始）
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// 移动到下一行
        /// </summary>
        /// <returns></returns>
        public CodePosition NextLine() {
            return new CodePosition {
                Line = Line + 1,
                Column = 0
            };
        }

        /// <summary>
        /// 移动到下一列
        /// </summary>
        /// <returns></returns>
        public CodePosition NextColumn() {
            return new CodePosition {
                Line = Line,
                Column = Column + 1
            };
        }

        /// <summary>
        /// 移动指定列数
        /// </summary>
        /// <param name="offset">要移动的距离</param>
        /// <returns></returns>
        public CodePosition MoveColumn(int offset) {
            return new CodePosition {
                Line = Line,
                Column = Column + offset
            };
        }

        /// <summary>
        /// 移动指定行数
        /// </summary>
        /// <param name="offset">要移动的距离</param>
        /// <returns></returns>
        public CodePosition MoveLine(int offset) {
            return new CodePosition {
                Line = Line + offset,
                Column = Column
            };
        }
    }
}