using System;
using Core.VisualNovel.Runtime.Variables.Values;

namespace Core.VisualNovel.Runtime.Variables {
    /// <summary>
    /// 表示一个变量
    /// </summary>
    public class Variable {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public IVariableValue Value { get => _value;
            set {
                if (IsConstant) throw new NotSupportedException("Cannot assign value to constant variable");
                _value = value;
            }
            
        }
        
        /// <summary>
        /// 获取或设置变量是否为常量
        /// </summary>
        public bool IsConstant { get; set; }

        private IVariableValue _value;

        /// <summary>
        /// 获取此变量的一个拷贝
        /// </summary>
        /// <returns></returns>
        public Variable Duplicate() {
            return new Variable {Value = Value.Duplicate()};
        }
    }
}