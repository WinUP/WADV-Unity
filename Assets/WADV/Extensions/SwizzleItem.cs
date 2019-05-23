namespace WADV.Extensions {
    /// <summary>
    /// 重组分量集合
    /// </summary>
    public enum SwizzleItem {
        /// <summary>
        /// X分量
        /// </summary>
        X,
        /// <summary>
        /// Y分量
        /// </summary>
        Y,
        /// <summary>
        /// Z分量
        /// </summary>
        Z,
        /// <summary>
        /// W分量
        /// </summary>
        W,
        /// <summary>
        /// 红色分量（等同于X）
        /// </summary>
        R = X,
        /// <summary>
        /// 绿色分量（等同于Y）
        /// </summary>
        G = Y,
        /// <summary>
        /// 蓝色分量（等同于Z）
        /// </summary>
        B = Z,
        /// <summary>
        /// 透明度分量（等同于W）
        /// </summary>
        A = W
    }
}