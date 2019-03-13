using System.Threading.Tasks;
using UnityEngine;
using WADV.MessageSystem;

namespace WADV.Plugins.Image {
    // [RequireComponent(typeof(Canvas))]
    public class ImageCanvas : MonoMessengerBehaviour {
        
        public override int Mask { get; } = 1;
        public override bool IsStandaloneMessage { get; } = false;
        
        public override Task<Message> Receive(Message message) {
            var rect = GetComponent<RectTransform>(); 
            Debug.Log($"{rect.anchoredPosition.x} {rect.anchoredPosition.y} {rect.rect.width} {rect.rect.height}");
            Debug.Log($"{rect.position.x} {rect.position.y} {rect.position.z}");
//            var image = new GameObject();
//            image.AddComponent<RectTransform>();
//            image.transform.SetParent(GetComponent<RectTransform>());
//            image.transform.SetSiblingIndex(400);
            return Task.FromResult(message);
        }
    }
}