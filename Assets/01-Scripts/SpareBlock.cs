using devlog98.Actor;
using UnityEngine;

namespace devlog98.Block {
    public class SpareBlock : MonoBehaviour {
        [SerializeField] private PlayerBlock spareBlock; // spare to be spawned
        [SerializeField] private SpriteRenderer renderer;

        // check collision with Player Blocks
        private void OnTriggerEnter2D(Collider2D collision) {
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