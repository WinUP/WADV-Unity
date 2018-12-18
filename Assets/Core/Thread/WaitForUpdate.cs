using UnityEngine;

namespace Core.Thread {
    public class WaitForUpdate : CustomYieldInstruction {
        public override bool keepWaiting { get; } = false;
    }
}