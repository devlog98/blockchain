using devlog98.Actor;
using UnityEngine;

namespace devlog98.Block {
    public class RotateBlock : MonoBehaviour {
        [SerializeField] private SpriteRenderer renderer;

        // check collision with Player Blocks
        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.tag == "Block") {
                PlayerBlock block = collision.gameObject.GetComponent<PlayerBlock>();
                Player.instance.StartRotation(block, renderer.color);
                Destroy(this.gameObject);
            }
        }
    }
}