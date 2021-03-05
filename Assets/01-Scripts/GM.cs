using devlog98.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace devlog98.GM {
    public class GM : MonoBehaviour {
        public static GM instance; // singleton
        private const float waitTime = .5f;
        [SerializeField] private AudioClip selectClip;

        // initialize singleton
        private void Awake() {
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
            }
            else {
                instance = this;
            }
        }

        public void ReloadScene() {
            AudioManager.instance.PlayOneShot(selectClip);
            int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneBuildIndex);
        }

        public void LoadScene(int sceneBuildIndex) {
            if (sceneBuildIndex == 0) {
                AudioManager.instance.PlayOneShot(selectClip);
            }

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