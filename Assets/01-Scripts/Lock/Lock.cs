using devlog98.Actor;
using devlog98.Level;
using UnityEngine;
using UnityEngine.Events;

/*
 * Trigger that checks key composition against Player composition
 */

namespace devlog98.Block {
    public class Lock : MonoBehaviour {
        [Header("Lock")]
        [SerializeField] private Key key;
        [SerializeField] private UnityEvent unlockEvent;
        private bool isLocked = true;

        private void OnTriggerStay2D(Collider2D collision) {
            if (isLocked) {
                if (collision.tag == "Block") {
                    if (key.CheckAgainstKeyBlocks(Player.instance.Blocks)) {
                        unlockEvent.Invoke();
                        Destroy(key.gameObject);
                        isLocked = false;
                    }
                }
            }
        }
    }
}