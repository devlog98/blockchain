using devlog98.Actor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Every single unit of the Player object
 * Checks for collisions depending on direction
 * Can be destroyed
 */

namespace devlog98.Block {
    public class PlayerBlock : MonoBehaviour {
        // reference to neighbour blocks
        public PlayerBlock rightBlock;
        public PlayerBlock leftBlock;
        public PlayerBlock upBlock;
        public PlayerBlock downBlock;

        public PlayerBlock RightBlock { get => rightBlock; }
        public PlayerBlock LeftBlock { get => leftBlock; }
        public PlayerBlock UpBlock { get => upBlock; }
        public PlayerBlock DownBlock { get => downBlock; }

        [Header("Collision")]
        [SerializeField] private float rayDistance;
        [SerializeField] private LayerMask rayMask;

        public void ExecuteStart() {
            rightBlock = CheckBlockOnDirection(PlayerDirection.Right);
            leftBlock = CheckBlockOnDirection(PlayerDirection.Left);
            upBlock = CheckBlockOnDirection(PlayerDirection.Up);
            downBlock = CheckBlockOnDirection(PlayerDirection.Down);
        }

        public void Update() {

        }

        // check for Player Blocks on specific direction
        private PlayerBlock CheckBlockOnDirection(PlayerDirection direction) {
            PlayerBlock block = null;

            // get ray direction
            Vector2 rayDirection = Vector2.zero;
            switch(direction) {
                case PlayerDirection.Right:
                    rayDirection = Vector2.right;
                    break;
                case PlayerDirection.Left:
                    rayDirection = Vector2.left;
                    break;
                case PlayerDirection.Up:
                    rayDirection = Vector2.up;
                    break;
                case PlayerDirection.Down:
                    rayDirection = Vector2.down;
                    break;
            }

            // cast ray on direction
            RaycastHit2D[] rayHits = Physics2D.RaycastAll(transform.position, rayDirection, rayDistance, rayMask);
            foreach(RaycastHit2D rayHit in rayHits) {
                if (rayHit.collider != null && rayHit.collider.gameObject != this.gameObject) {
                    block = rayHit.collider.gameObject.GetComponent<PlayerBlock>();
                }
            }

            return block;
        }
    }
}