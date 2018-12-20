using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Extensions;

namespace Core.VisualNovel.Translation {
    /// <summary>
    /// 表示一个可翻译字符串列表
    /// </summary>
    public class ScriptTranslation {
        /// <summary>
        /// 翻译不存在时的替代字符串
        /// </summary>
        public const string MissingTranslation = "<MISSING ㄟ( ▔, ▔ )ㄏ TRANSLATION>";
        private readonly Dictionary<uint, string> _translatableStrings = new Dictionary<uint, string>();
        
        /// <summary>
        /// 从可移动字符串新建可翻译字符串列表
        /// </summary>
        /// <param name="content">可移动字符串内容</param>
        public ScriptTranslation(string content) {
            var fileContent = (from e in content.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n') where !e.StartsWith("//") select e.Trim()).ToArray();
            for (var i = -1; ++i < fileContent.Length;) {
                var line = fileContent[i];
                if (line.Length == 0) {
                    continue;
                }
                var idList = line.Split(':').Select(e => e.Length == 8 ? Convert.ToUInt32(e, 16) : throw new ArgumentException($"Translate file format error: {e} is not valid string id"));
                ++i;
                line = fileContent[i];
                foreach (var id in idList) {
                    _translatableStrings.Add(id, line);
                }
            }
        }
        
        /// <summary>
        /// 从集合新建可翻译字符串列表
        /// </summary>
        /// <param name="content">来源集合</param>
        public ScriptTranslation(IReadOnlyDictionary<uint, string> content) {
            foreach (var (id, value) in content) {
                _translatableStrings.Add(id, value);
            }
        }

        /// <summary>
        /// 获取翻译
        /// <para>找不到合适的翻译时会返回<code>ScriptTranslation.MissingTranslation</code>替代字符串</para>
        /// </summary>
        /// <param name="id">翻译字符串ID</param>
        /// <returns></returns>
        public string GetTranslation(uint id) {
            return _translatableStrings.ContainsKey(id) ? _translatableStrings[id] : MissingTranslation;
        }

        /// <summary>
        /// 将新的翻译合并到此翻译中
        /// <list type="bullet">
        ///     <item><description>目标与此翻译ID和哈希相同的条目会被忽略</description></item>
        ///     <item><description>目标ID不同但哈希于此翻译中存在的条目会使用此翻译中同哈希的第一个条目的内容</description></item>
        ///     <item><description>目标与此翻译哈希不同的条目会在此翻译中新建</description></item>
        ///     <item><description>此翻译中存在但是目标中不存在的条目不会处理</description></item>
        /// </list>
        /// </summary>
        /// <param name="source">目标翻译</param>
        public bool MergeWith(ScriptTranslation source) {
            var changed = false;
            foreach (var (key, content) in source._translatableStrings) {
                // 情况1
                if (_translatableStrings.ContainsKey(key)) {
                    continue;
                }
                // 情况2
                var similarIds = _translatableStrings.Keys.Where(e => e << 16 == key << 16).ToArray();
                if (similarIds.Any()) {
                    _translatableStrings.Add(key, _translatableStrings[similarIds.First()]);
                    changed = true;
                    continue;
                }
                // 情况3
                _translatableStrings.Add(key, content);
                changed = true;
            }
            return changed;
        }

        /// <summary>
        /// 删除此翻译中存在但是目标中不存在的条目
        /// </summary>
        /// <param name="baseTranslation">目标翻译</param>
        /// <returns></returns>
        public bool RemoveUnavailableTranslations(ScriptTranslation baseTranslation) {
            var needRemove = new List<uint>(_translatableStrings.Keys.Where(e => !baseTranslation._translatableStrings.ContainsKey(e)));
            foreach (var id in needRemove) {
                _translatableStrings.Remove(id);
            }
            return needRemove.Count > 0;
        }

        public byte[] ToByteArray() {
            var writer = new BinaryWriter(new MemoryStream());
            writer.Write(_translatableStrings.Count);
            foreach (var item in _translatableStrings) {
                writer.Write(item.Key);
                writer.Write(item.Value);
            }
            var result = ((MemoryStream) writer.BaseStream).ToArray();
            writer.Close();
            return result;
        }

        /// <summary>
        /// 将此翻译输出为可移动字符串
        /// </summary>
        /// <returns></returns>
        public string Pack() {
            var content = new StringBuilder();
            foreach (var group in _translatableStrings.GroupBy(e => e.Value)) {
                content.AppendLine(string.Join(":", group.Select(e => Convert.ToString(e.Key, 16).PadLeft(8, '0'))));
                content.AppendLine(group.Key.PackEscapeCharacters());
                content.AppendLine();
            }
            return content.ToString();
        }
    }
}