namespace Assets.Core.VisualNovel.Script.Compiler {
    public struct CodePosition {
        public int Line { get; set; }
        public int Column { get; set; }

        public CodePosition NextLine() {
            return new CodePosition {
                Line = Line + 1,
                Column = 0
            };
        }

        public CodePosition NextColumn() {
            return new CodePosition {
                Line = Line,
                Column = Column + 1
            };
        }

        public CodePosition MoveColumn(int offset) {
            return new CodePosition {
                Line = Line,
                Column = Column + offset
            };
        }

        public CodePosition MoveLine(int offset) {
            return new CodePosition {
                Line = Line + offset,
                Column = Column
            };
        }
    }
}