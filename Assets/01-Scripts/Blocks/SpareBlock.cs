using devlog98.Actor;
using UnityEngine;

/*
 * When Player touches this block, it receives an additional block in the correspondent position
 */

namespace devlog98.Block {
    public class SpareBlock : BaseBlock {
        [SerializeField] private PlayerBlock spareBlock; // spare to be spawned

        // check collision with Player Blocks
        public override void OnTriggerEnter2D(Collider2D collision) {
            if (collision.tag == "Block") {
                // spawn spare block
                spareBlock.gameObject.SetActive(true);
                spareBlock.transform.parent = null;

                // add to Player list
                PlayerBlock block = collision.gameObject.GetComponent<PlayerBlock>();
                Player.instance.AddBlock(spareBlock, block, renderer.color);

                Destroy(this.gameObject);
            }
        }
    }
}