using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.MessageSystem {
    [DisallowMultipleComponent]
    public class MessageListener : MonoBehaviour {
        /// <summary>
        /// 在协程中处理消息
        /// </summary>
        /// <param name="message">目标消息</param>
        /// <param name="listenerRoot">消息接收器链接树的根节点</param>
        public void Process(Message message, LinkedTreeNode<IMessenger> listenerRoot) {
            StartCoroutine(ProcessMessage(message, listenerRoot));
        }

        private void OnDestroy() {
            MessageService.UseListener(this);
        }

        private void OnDisable() {
            MessageService.CancelListener(this);
        }

        private void OnEnable() {
            MessageService.CancelListener(this);
        }

        private void Start() {
            MessageService.UseListener(this);
        }

        private IEnumerator ProcessMessage(Message message, IEnumerable<IMessenger> listenerRoot) {
            foreach (var receiver in listenerRoot) {
                if ((receiver.Mask & message.Mask) == 0) {
                    continue;
                }
                yield return StartCoroutine(receiver.Receive(message));
            }
            yield return message;
        }
    }
}