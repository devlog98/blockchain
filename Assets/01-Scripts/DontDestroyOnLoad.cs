using UnityEngine;

namespace devlog98.Level {
    public class DontDestroyOnLoad : MonoBehaviour {
        [SerializeField] private string tag;

        private void Awake() {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            if (objs.Length > 1) {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }
}