using UnityEngine;

/*
 * The game object where this script is placed will not be destroyed when loading new scenes
 * You must insert a tag in order to check and delete repeated objects
 */

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