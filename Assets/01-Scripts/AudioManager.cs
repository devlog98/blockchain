using UnityEngine;

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