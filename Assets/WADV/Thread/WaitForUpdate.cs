using UnityEngine;

namespace WADV.Thread {
    public class WaitForUpdate : CustomYieldInstruction {
        private static WaitForUpdate _instance;

        public static WaitForUpdate Instance {
            get {
                _instance = _instance ?? new WaitForUpdate();
                return _instance;
            }
        }
        
        public override bool keepWaiting { get; } = false;
    }
}