using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace devlog98.GM {
    public class GM : MonoBehaviour {
        public static GM instance; // singleton
        private const float waitTime = .7f;

        // initialize singleton
        private void Awake() {
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
            }
            else {
                instance = this;
            }
        }

        public void LoadScene(int sceneBuildIndex) {
            StopAllCoroutines();
            StartCoroutine(WaitCoroutine(sceneBuildIndex));
        }

        public void ReloadScene() {
            int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            StopAllCoroutines();
            StartCoroutine(WaitCoroutine(sceneBuildIndex));
        }

        public void Quit() {
            StopAllCoroutines();
            StartCoroutine(WaitCoroutine(-1));
        }

        private IEnumerator WaitCoroutine(int sceneBuildIndex) {
            yield return new WaitForSeconds(waitTime);

            if (sceneBuildIndex >= 0) {
                SceneManager.LoadScene(sceneBuildIndex);
            }
            else {
                Application.Quit();
            }
        }
    }
}