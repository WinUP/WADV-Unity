using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WADV.Thread {
    public class WaitForBackgroundThread {
        private static WaitForBackgroundThread _instance;

        public static WaitForBackgroundThread Instance {
            get {
                _instance = _instance ?? new WaitForBackgroundThread();
                return _instance;
            }
        }
        
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter() {
            return Task.Run(() => {}).ConfigureAwait(false).GetAwaiter();
        }
    }
}