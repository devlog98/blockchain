using devlog98.Actor;
using UnityEngine;

/*
 * When Player touches this block, it can rotate to either sides
 */

namespace devlog98.Block {
    public class RotateBlock : BaseBlock {
        // check collision with Player Blocks
        public override void OnTriggerEnter2D(Collider2D collision) {
            if (collision.tag == "Block") {
                PlayerBlock block = collision.gameObject.GetComponent<PlayerBlock>();
                Player.instance.StartRotation(block, renderer.color);
                Destroy(this.gameObject);
            }
        }
    }
}