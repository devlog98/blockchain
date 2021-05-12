using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/*
 * Activates post process based on memory budget
 */

namespace devlog98.Platforms {
    public class PostProcessManager : MonoBehaviour {
        [SerializeField] private PostProcessVolume volume;

        private void Awake() {
            // At least 2GB of VRAM
            if (SystemInfo.graphicsMemorySize >= 2048) {
                volume.enabled = true;
            }
        }
    }
}