using System.Threading.Tasks;
using UnityEngine;
using WADV.MessageSystem;

namespace WADV.Plugins.Image {
    [RequireComponent(typeof(Canvas))]
    public class ImageCanvas : MonoMessengerBehaviour {
        
        public override int Mask { get; } = 1;
        public override bool IsStandaloneMessage { get; } = false;
        
        public override Task<Message> Receive(Message message) {
            var image = new GameObject();
            image.AddComponent<RectTransform>();
            image.transform.SetParent(GetComponent<RectTransform>());
            image.transform.SetSiblingIndex(400);
            return Task.FromResult(message);
        }
    }
}