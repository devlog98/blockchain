using UnityEngine;

/*
 * Abstract class used for interactable blocks
 * Besides the Player Block, all blocks should derive from
 */

namespace devlog98.Block {
    public abstract class BaseBlock : MonoBehaviour {
        [SerializeField] protected SpriteRenderer renderer; // visual aspect of block
        protected const string collisionTag = "Block"; // tag to be checked against on collision

        // check collisions (normally with Player Blocks)
        public abstract void OnTriggerEnter2D(Collider2D collision);
    }
}