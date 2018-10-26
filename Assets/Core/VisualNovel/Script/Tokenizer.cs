using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.VisualNovel.Script {
    public class Tokenizer {
        private readonly string _file;
        private string _path;

        public Tokenizer(string path) {
            _path = path;
            var asset = Resources.Load<TextAsset>(path);
            _file = asset.text.Replace("\r\n", "\n").Replace('\r', '\n');
            Resources.UnloadAsset(asset);
        }

        public IEnumerable<Token> Analyze() {
            var pointer = 0;
            var totalLength = _file.Length;
            var tokens = new List<Token>();
            var state = new List<TokenizerState>();
            var lineNumber = 1;
            var cache = new StringBuilder();
            while (pointer < totalLength) {
                var character = _file[pointer];
                var next = pointer == totalLength - 1 ? '\0' : _file[pointer + 1];
                // 换行
                if (character == '\n') {
                    if (state.Last() == TokenizerState.InDialogue) {
                        state.RemoveAt(state.Count - 1);
                        tokens.Add(new Token(TokenType.DialogueEnd, lineNumber, pointer + 1));
                    }
                    lineNumber++;
                }
                // 对话
                else if (character == '#') {
                    tokens.Add(new Token(TokenType.DialogueStart, lineNumber, pointer + 1));
                    state.Add(TokenizerState.InDialogue);
                    var lineEnd = _file.IndexOf('\n', pointer);
                    var index = _file.IndexOf(' ', pointer);
                    if (lineEnd == pointer + 1) {
                        tokens.Add(new Token(TokenType.DialogueContentStart, lineNumber, pointer + 1));
                        tokens.Add(new Token(TokenType.String, lineNumber, pointer + 1));
                        tokens.Add(new Token(TokenType.DialogueContentEnd, lineNumber, pointer + 1));
                    }
                    if (index < lineEnd) {
                        tokens.Add(new Token(TokenType.DialogueNameStart, lineNumber, pointer + 1));
                        state.Add(TokenizerState.InDialogueName);
                    } else {
                        tokens.Add(new Token(TokenType.DialogueContentStart, lineNumber, pointer + 1));
                        state.Add(TokenizerState.InDialogueContent);
                    }
                }
                // 注释
                else if (character == '/' && next == '/') {
                    var index = _file.IndexOf('\n', pointer + 1);
                    if (index <= pointer) {
                        return tokens;
                    }
                    pointer = index;
                }
                // 转义
                else if (character == '\\') {
                    if (next != '\0') {
                        cache.Append(next);
                    }
                    pointer++;
                }
                // 制表符
                else if (character == '\t') {
                    if (state.Contains(TokenizerState.InDialogue)) {
                        cache.Append(character);
                    } else {
                        throw new TokenizerException(_path, lineNumber, pointer + 1, "WADV VNS file cannot contains \\t outside of dialogue");
                    }
                }
                // 指令
                else if (character == '[') {
                    state.Add((TokenizerState.InCommand);
                    state.Add((TokenizerState.InCommandName);
                    tokens.Add(new Token(TokenType.CommandStart, lineNumber, pointer + 1));
                    tokens.Add(new Token(TokenType.CommandNameStart, lineNumber, pointer + 1));
                }
                // 变量
                else if (character == '@') {
                    if (state.Contains(TokenizerState.InDialogue)) {
                        cache.Append(character);
                    } else {
                        tokens.Add(new Token(TokenType.DialogueStart, lineNumber, pointer + 1));
                        state.Add(TokenizerState.InDialogue);
                    }
                }
                // 空格
                else if (character == ' ') {
                    if (state.Contains(TokenizerState.InVariableName)) {
                        if (cache.Length > 0) {
                            tokens.Add(new Token(TokenType.String, lineNumber, pointer + 1, cache.ToString()));
                            cache.Clear();
                        }
                        tokens.Add(new Token(TokenType.VariableEnd, lineNumber, pointer + 1, cache.ToString()));
                        state.RemoveAt(state.LastIndexOf(TokenizerState.InVariableName));
                    }
                    if (state.Contains(TokenizerState.InCommandName)) {
                        tokens.Add(new Token(TokenType.CommandNameStart, lineNumber, pointer + 1, cache.ToString()));
                        cache.Clear();
                        state.RemoveAt(state.Count - 1);
                    }
                    if (state.Contains(TokenizerState.InDialogueName)) {
                        cache.Append(character);
                    }
                }
                // 一般文本
                else {
                    cache.Append(character);
                }
                pointer++;
            }
            return tokens;
        }
    }
}
