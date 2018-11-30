using System;
using System.Collections;
using Core;
using Core.VisualNovel;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Core.VisualNovel {
    public class VisualNetwork : MonoBehaviour, IMessenger {
        /// <summary>
        /// 获取最近一个执行结束的指令
        /// </summary>
        public VisualCommand CurrentCommand { get; private set; }

        public int Mask { get; } = MessageMask.VisualNetwork;
        public bool Awaking { get; set; }

        public Message Receive(Message message) {
            switch (message.Tag) {
                case MessageTag.RunCommand:
                    Run(((Message<VisualCommand>)message).Content);
                    break;
                case MessageTag.NextCommand:
                    Next();
                    break;
            }
            return message;
        }

        [UsedImplicitly]
        public void Start() {
            MessageService.Receivers.Add(this);
        }

        [UsedImplicitly]
        public void OnEnable() {
            Awaking = true;
        }

        [UsedImplicitly]
        public void OnDisable() {
            Awaking = false;
        }

        [UsedImplicitly]
        public void OnDestroy() {
            MessageService.Receivers.Remove(this);
        }

        /// <summary>
        /// 从一个节点启动指令网络
        /// </summary>
        /// <param name="command">入口节点</param>
        public void Run(VisualCommand command) {
            var root = new EmptyVisualCommand(Guid.NewGuid().ToString());
            root.NextCommand.Add(command);
            CurrentCommand = root;
            Next();
        }

        /// <summary>
        /// 执行下一个指令
        /// </summary>
        public void Next() {
            StartCoroutine(RunCommand());
        }

        /// <summary>
        /// 以指定指令为入口搜索指令网络中具有目标ID的指令
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <param name="entrance">入口指令</param>
        /// <returns></returns>
        public static VisualCommand FindCommand(string id, VisualCommand entrance) {
            if (entrance.ID == id) {
                return entrance;
            }
            foreach (var command in entrance.NextCommand) {
                var result = FindCommand(id, command);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }

        private IEnumerator RunCommand() {
            var nextCommand = CurrentCommand.Next();
            yield return StartCoroutine(nextCommand.Run());
            CurrentCommand = nextCommand;
        }
    }
}
