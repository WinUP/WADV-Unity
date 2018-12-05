using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Extensions;

namespace Core.VisualNovel.Translation {
    public class ScriptTranslation {
        public const string MissingTranslation = "<MISSING ㄟ( ▔, ▔ )ㄏ TRANSLATION>";
        private readonly Dictionary<int, string> _translatableStrings = new Dictionary<int, string>();
        
        public ScriptTranslation(string content) {
            var fileContent = (from e in content.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n') where !e.StartsWith("//") select e.Trim()).ToArray();
            for (var i = -1; ++i < fileContent.Length;) {
                var line = fileContent[i];
                if (line.Length == 0) {
                    continue;
                }
                var idList = line.Split(':').Select(e => e.Length == 8 ? Convert.ToInt32(e, 16) : throw new ArgumentException($"Translate file format error: {e} is not valid string id"));
                ++i;
                line = fileContent[i];
                foreach (var id in idList) {
                    _translatableStrings.Add(id, line);
                }
            }
        }
        
        public ScriptTranslation(IReadOnlyDictionary<int, string> content) {
            foreach (var pair in content) {
                _translatableStrings.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// 获取翻译
        /// <para>找不到合适的翻译时会返回<code>ScriptTranslation.MissingTranslation</code>替代字符串</para>
        /// </summary>
        /// <param name="id">翻译字符串ID</param>
        /// <returns></returns>
        public string GetTranslation(int id) {
            return _translatableStrings.ContainsKey(id) ? _translatableStrings[id] : MissingTranslation;
        }

        /// <summary>
        /// 将新的翻译合并到此翻译中
        /// <para>目标与此翻译ID相同的条目会被忽略</para>
        /// <para>目标与此翻译ID和哈希相同的条目会被忽略</para>
        /// <para>目标与此翻译ID与哈希完全不同的条目会在此翻译中新建</para>
        /// <para>此翻译中存在但是目标中不存在的条目的处理方式取决于函数第二个参数</para>
        /// </summary>
        /// <param name="target">目标翻译</param>
        /// <param name="removeUnavailableTranslations">是否删除所有不在目标但是存在于此翻译中的条目</param>
        public void MergeWith(ScriptTranslation target, bool removeUnavailableTranslations = false) {
            MergeWith(target._translatableStrings, removeUnavailableTranslations);
        }

        /// <summary>
        /// 将新的翻译合并到此翻译中
        /// <para>目标与此翻译ID和哈希相同的条目会被忽略</para>
        /// <para>目标ID不同但哈希于此翻译中存在的条目会使用此翻译中同哈希的第一个条目的内容</para>
        /// <para>目标与此翻译ID与哈希完全不同的条目会在此翻译中新建</para>
        /// <para>此翻译中存在但是目标中不存在的条目的处理方式取决于函数第二个参数</para>
        /// </summary>
        /// <param name="source">目标翻译</param>
        /// <param name="removeUnavailableTranslations">是否删除所有不在目标但是存在于此翻译中的条目</param>
        public void MergeWith(IReadOnlyDictionary<int, string> source, bool removeUnavailableTranslations = false) {
            var proceedIds = new List<int>();
            foreach (var newTranslation in source) {
                // 情况1
                if (_translatableStrings.ContainsKey(newTranslation.Key)) {
                    proceedIds.Add(newTranslation.Key);
                    continue;
                }
                // 情况2
                var similarIds = _translatableStrings.Keys.Where(e => e << 16 == newTranslation.Key << 16).ToArray();
                if (similarIds.Any()) {
                    proceedIds.Add(newTranslation.Key);
                    _translatableStrings.Add(newTranslation.Key, _translatableStrings[similarIds.First()]);
                    continue;
                }
                // 情况3
                _translatableStrings.Add(newTranslation.Key, newTranslation.Value);
                proceedIds.Add(newTranslation.Key);
            }
            // 删除多余条目（可选）
            if (!removeUnavailableTranslations) return;
            foreach (var id in _translatableStrings.Keys.Where(e => !proceedIds.Contains(e))) {
                _translatableStrings.Remove(id);
            }
        }

        /// <summary>
        /// 将此翻译输出为单个字符串
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