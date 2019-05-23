using System;

namespace WADV.Extensions {
    /// <summary>
    /// 区间类型
    /// </summary>
    [Flags]
    public enum IntervalType {
        /// <summary>
        /// 开区间
        /// </summary>
        Open = 0b0101,
        /// <summary>
        /// 闭区间
        /// </summary>
        Closed = 0b1010,
        /// <summary>
        /// 左开右闭区间
        /// </summary>
        OpenClosed = 0b0110,
        /// <summary>
        /// 左闭右开区间
        /// </summary>
        ClosedOpen = 0b1001
    }
}