using UnityEngine;

/*
 * Singleton responsible for playing all sound effects of the game
 */

namespace devlog98.Audio {
    public class AudioManager : MonoBehaviour {
        public static AudioManager instance; // singleton

        [SerializeField] private AudioSource source;

        // initialize singleton
        private void Awake() {
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
            }
            else {
                instance = this;
            }
        }

        public void PlayOneShot(AudioClip clip) {
            source.PlayOneShot(clip);
        }
    }
}