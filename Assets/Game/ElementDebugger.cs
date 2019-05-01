using UnityEngine;

namespace Game {
    public class ElementDebugger : MonoBehaviour {
        public int sibling;
        
        private void Update() {
            sibling = transform.GetSiblingIndex();
        }
    }
}