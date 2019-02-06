using System;
using JetBrains.Annotations;

namespace WADV.VisualNovel.ScriptStatus {
    /// <summary>
    /// 表示一个运行时可加载文件的路径
    /// </summary>
    [Serializable]
    public struct RelativePath {
        /// <summary>
        /// 运行时路径
        /// </summary>
        [CanBeNull] public string runtime;
        /// <summary>
        /// Asset资源路径
        /// </summary>
        public string asset;
    }
}