using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.VisualNovel.Script {
    public class Tokenizer {
        private readonly string _file;
        private string _path;

        public Tokenizer(string path) {
            _path = path;
            var asset = Resources.Load<TextAsset>(path);
            _file = asset.text.Replace("\r\n", "\n").Replace('\r', '\n');
            if (_file.Last() != '\n') {
                Debug.LogWarning($"VNS file {path}: Recommend to end with new empty line");
                _file += '\n';
            }
            Resources.UnloadAsset(asset);
        }

        public IEnumerable<Token> Analyze() {
            var position = 0;
            var totalLength = _file.Length;
            var tokens = new List<Token>();
            var state = new List<TokenizerState>();
            var line = 0;
            var cache = new StringBuilder();
            while (position < totalLength) {
                var character = _file[position];
                var next = position == totalLength - 1 ? '\0' : _file[position + 1];
                if (character == '/' && next == '/' && !state.Contains(TokenizerState.InDialogue)) {
                    var index = _file.IndexOf('\n', position + 1);
                    if (index <= position) {
                        return tokens;
                    }
                    position = index;
                } else if (character == '\n') {
                    if (state.Contains(TokenizerState.InDialogueName)) {
                        throw new TokenizerException(_path, line, position, "Dialogue must has content");
                    } else if (state.Contains(TokenizerState.InDialogueContent)) {
                        tokens.Add(new Token(TokenType.String, line, position - cache.Length, cache.ToString()));
                        tokens.Add(new Token(TokenType.DialogueContentEnd, line, position));
                        tokens.Add(new Token(TokenType.DialogueEnd, line, position));
                        cache.Clear();
                        state.Remove(TokenizerState.InDialogueContent);
                        state.Remove(TokenizerState.InDialogue);
                    }
                    line++;
                } else if (character == '#') {
                    if (state.Contains(TokenizerState.InDialogueContent)) {
                        cache.Append(character);
                    } else {
                        tokens.Add(new Token(TokenType.DialogueStart, line, position));
                        state.Add(TokenizerState.InDialogue);
                        tokens.Add(new Token(TokenType.DialogueNameStart, line, position));
                        state.Add(TokenizerState.InDialogueName);
                    }
                } else if (character == ' ') {
                    if (state.Contains(TokenizerState.InDialogueName)) {
                        tokens.Add(new Token(TokenType.String, line, position - cache.Length, cache.ToString()));
                        tokens.Add(new Token(TokenType.DialogueNameEnd, line, position));
                        tokens.Add(new Token(TokenType.DialogueContentStart, line, position));
                        cache.Clear();
                        state.Remove(TokenizerState.InDialogueName);
                        state.Add(TokenizerState.InDialogueContent);
                    } else {
                        cache.Append(character);
                    }
                }
                else {
                    if (!state.Contains(TokenizerState.InDialogue)) {
                        tokens.Add(new Token(TokenType.DialogueStart, line, position));
                        tokens.Add(new Token(TokenType.DialogueContentStart, line, position));
                        state.Add(TokenizerState.InDialogue);
                        state.Add(TokenizerState.InDialogueContent);
                    }
                    cache.Append(character);
                }
                position++;
            }
            return tokens;
        }
    }
}
