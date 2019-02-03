using System;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace WADV.VisualNovel.ScriptStatus {
    /// <summary>
    /// 表示一个运行时可加载文件的路径
    /// </summary>
    [Serializable]
    public struct RelativePath {
        /// <summary>
        /// 运行时路径
        /// </summary>
        [CanBeNull] public string Runtime;
        /// <summary>
        /// Asset资源路径
        /// </summary>
        public string Asset;
    }
}