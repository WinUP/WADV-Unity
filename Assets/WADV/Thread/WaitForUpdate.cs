using UnityEngine;

namespace WADV.Thread {
    public class WaitForUpdate : CustomYieldInstruction {
        public override bool keepWaiting { get; } = false;
    }
}