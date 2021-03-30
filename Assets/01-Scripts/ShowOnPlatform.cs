using UnityEngine;

/*
 * Shows or hides game objects based on specific device of the game
 */

namespace devlog98.Platforms {
    public enum PlatformType { Standalone, Mobile}

    public class ShowOnPlatform : MonoBehaviour {
        [SerializeField] private PlatformType platform;

        private void Start() {
            #if UNITY_STANDALONE
                if (platform == PlatformType.Standalone) {
                    this.gameObject.SetActive(true);
                }
                else {
                    this.gameObject.SetActive(false);
                }
            #endif

            #if UNITY_ANDROID
                if (platform == PlatformType.Mobile) {
                    this.gameObject.SetActive(true);
                }
                else {
                    this.gameObject.SetActive(false);
                }
            #endif
        }
    }
}