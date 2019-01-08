using UnityEngine;

namespace WADV.VisualNovel.Runtime {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个二进制脚本文件
    /// </summary>
    public class ScriptAsset : ScriptableObject {
        /// <summary>
        /// 二进制脚本内容
        /// </summary>
        public byte[] content;
        /// <summary>
        /// 脚本ID
        /// </summary>
        public string id;
    } 
}