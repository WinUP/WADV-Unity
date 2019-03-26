using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.MessageSystem;
using WADV.Plugins.Image.Utilities;

namespace WADV.Plugins.Image {
    [RequireComponent(typeof(Canvas))]
    public class ImageCanvas : MonoMessengerBehaviour {
        private Dictionary<ImageProperties, RawImage> _images = new Dictionary<ImageProperties, RawImage>();
        private Canvas _root;

        public override int Mask { get; } = ImageMessageIntegration.Mask;
        public override bool IsStandaloneMessage { get; } = false;

        private void Start() {
            _root = GetComponent<Canvas>();
        }

        public override async Task<Message> Receive(Message message) {
            if (message.HasTag(ImageMessageIntegration.ShowImage) && message is Message<ImageMessageIntegration.ShowImageContent> showMessage) {
                if (showMessage.Content.Effect == null) {
                    
                } else {
                    
                }
            }
            return message;
        }
    }
}