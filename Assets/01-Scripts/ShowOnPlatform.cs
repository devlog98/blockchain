using UnityEngine;

/*
 * Shows or hides game objects based on specific device of the game
 */

namespace devlog98.Platforms {
    public class ShowOnPlatform : MonoBehaviour {
        [SerializeField] private bool standalone;
        [SerializeField] private bool mobile;
        [SerializeField] private bool webGL;

        private void Start() {
            #if UNITY_STANDALONE
                this.gameObject.SetActive(standalone);
            #endif

            #if UNITY_ANDROID
                this.gameObject.SetActive(mobile);
            #endif

            #if UNITY_WEBGL
                this.gameObject.SetActive(webGL);
            #endif
        }
    }
}