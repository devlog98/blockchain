using devlog98.Actor;
using devlog98.Level;
using UnityEngine;

namespace devlog98.Block {
    public class LockBlock : MonoBehaviour {
        [Header("Lock")]
        [SerializeField] private Key key;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.tag == "Block") {
                if (key.CheckAgainstKeyBlocks(Player.instance.Blocks)) {
                    Debug.Log("Desbloqueou a fase!!!");
                }
                else {
                    Debug.Log("Errou a composição!!!");
                }
            }
        }
    }
}