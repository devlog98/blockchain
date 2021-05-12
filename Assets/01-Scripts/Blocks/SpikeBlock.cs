using devlog98.Actor;
using UnityEngine;

/*
 * When Player touches this block, it loses a block
 */

namespace devlog98.Block {
    public class SpikeBlock : BaseBlock {
        // check collision with Player Blocks
        public override void OnTriggerEnter2D(Collider2D collision) {
            if (collision.tag == "Block") {
                PlayerBlock block = collision.gameObject.GetComponent<PlayerBlock>();
                Player.instance.DestroyBlock(block, renderer.color);
                Destroy(this.gameObject);
            }
        }
    }
}