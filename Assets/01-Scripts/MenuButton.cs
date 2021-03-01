using devlog98.Actor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace devlog98.Menu {
    public class MenuButton : MonoBehaviour {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Color activationColor;
        [SerializeField] private UnityEvent activationEvent;
        private bool isActivated;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (!isActivated) {
                if (collision.tag == "Block") {
                    tilemap.color = activationColor;
                    activationEvent.Invoke();
                }
            }
        }
    }
}