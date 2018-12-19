namespace Core.VisualNovel.Compiler {
    /// <summary>
    /// 表示一个源代码坐标
    /// </summary>
    public struct SourcePosition {
        /// <summary>
        /// 行号（从0开始）
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// 列号（从0开始）
        /// </summary>
        public int Column { get; private set; }

        public static readonly SourcePosition UnavailablePosition = new SourcePosition {Line = -1, Column = -1};

        /// <summary>
        /// 新建一个源代码坐标
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static SourcePosition Create(int line, int column) {
            return new SourcePosition {Line = line, Column = column};
        }

        /// <summary>
        /// 移动到下一行
        /// </summary>
        /// <returns></returns>
        public SourcePosition NextLine() {
            return new SourcePosition {
                Line = Line + 1,
                Column = 0
            };
        }

        /// <summary>
        /// 移动到下一列
        /// </summary>
        /// <returns></returns>
        public SourcePosition NextColumn() {
            return new SourcePosition {
                Line = Line,
                Column = Column + 1
            };
        }

        /// <summary>
        /// 移动指定列数
        /// </summary>
        /// <param name="offset">要移动的距离</param>
        /// <returns></returns>
        public SourcePosition MoveColumn(int offset) {
            return new SourcePosition {
                Line = Line,
                Column = Column + offset
            };
        }

        /// <summary>
        /// 移动指定行数
        /// </summary>
        /// <param name="offset">要移动的距离</param>
        /// <returns></returns>
        public SourcePosition MoveLine(int offset) {
            return new SourcePosition {
                Line = Line + offset,
                Column = Column
            };
        }
    }
}