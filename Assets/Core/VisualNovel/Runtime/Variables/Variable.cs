using Core.VisualNovel.Runtime.Variables.Values;

namespace Core.VisualNovel.Runtime.Variables {
    /// <summary>
    /// 表示一个变量
    /// </summary>
    public class Variable {
        public IVariableValue Value { get; set; }

        /// <summary>
        /// 获取此变量的一个拷贝
        /// </summary>
        /// <returns></returns>
        public Variable Duplicate() {
            return new Variable {Value = Value.Duplicate()};
        }
    }
}