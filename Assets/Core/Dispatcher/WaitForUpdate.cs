using UnityEngine;

namespace Core.Dispatcher {
    public class WaitForUpdate : CustomYieldInstruction {
        public override bool keepWaiting { get; } = false;
    }
}