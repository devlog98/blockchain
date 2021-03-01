using devlog98.Actor;
using UnityEngine;

/*
 * Every single unit of the Player object
 * Checks for collisions depending on direction
 * Can be destroyed
 */

namespace devlog98.Block {
    public class PlayerBlock : MonoBehaviour {        
        public PlayerBlock rightBlock; // reference to neighbour blocks
        public PlayerBlock leftBlock;
        public PlayerBlock upBlock;
        public PlayerBlock downBlock;

        public PlayerBlock RightBlock { get => rightBlock; }
        public PlayerBlock LeftBlock { get => leftBlock; }
        public PlayerBlock UpBlock { get => upBlock; }
        public PlayerBlock DownBlock { get => downBlock; }        

        [Header("Collision")]
        [SerializeField] private float collisionDistance;
        [SerializeField] private float collisionRaySpacing;
        [SerializeField] private LayerMask blockMask;
        [SerializeField] private LayerMask wallMask;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer renderer;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Sprite rotateBlockSprite;
        [SerializeField] private Color rotateBlockColor;
        public Color RotateBlockColor { get => rotateBlockColor; }

        // get block neighbours
        public void CheckBlockNeighbours() {
            GameObject block;
            PlayerBlock[] blocks = new PlayerBlock[4];
            PlayerDirection[] directions = { PlayerDirection.Right, PlayerDirection.Left, PlayerDirection.Up, PlayerDirection.Down };

            for (int i = 0; i < blocks.Length; i++) {
                block = CheckCollisionOnDirection(directions[i], true);
                if (block != null) {
                    blocks[i] = block.GetComponent<PlayerBlock>();
                }
            }

            rightBlock = blocks[0];
            leftBlock = blocks[1];
            upBlock = blocks[2];
            downBlock = blocks[3];
        }

        // check for Player Blocks on specific direction
        public GameObject CheckCollisionOnDirection(PlayerDirection direction, bool blockCheck) {
            GameObject gameObject = null;

            // get ray direction
            Vector3 rayDirection = Vector3.zero;
            Vector3 raySpacingDirection = Vector3.zero;
            switch (direction) {
                case PlayerDirection.Right:
                    rayDirection = Vector3.right;
                    raySpacingDirection = Vector3.up;
                    break;
                case PlayerDirection.Left:
                    rayDirection = Vector3.left;
                    raySpacingDirection = Vector3.up;
                    break;
                case PlayerDirection.Up:
                    rayDirection = Vector3.up;
                    raySpacingDirection = Vector3.right;
                    break;
                case PlayerDirection.Down:
                    rayDirection = Vector3.down;
                    raySpacingDirection = Vector3.right;
                    break;
            }

            // cast ray on direction
            LayerMask rayMask = (blockCheck) ? blockMask : wallMask;
            for(int i = -1; i <= 1; i++) {
                Debug.DrawRay(transform.position + (raySpacingDirection * collisionRaySpacing * i), rayDirection * collisionDistance, Color.red);
                RaycastHit2D[] rayHits = Physics2D.RaycastAll(transform.position + (raySpacingDirection * collisionRaySpacing * i), rayDirection, collisionDistance, rayMask);
                foreach (RaycastHit2D rayHit in rayHits) {
                    if (rayHit.collider != null && rayHit.collider.gameObject != this.gameObject) {
                        gameObject = rayHit.collider.gameObject;
                        break;
                    }
                }
            }

            return gameObject;
        }

        // when Player Block becomes or stops being a pivot
        public void SetAsPivot() {
            renderer.sprite = rotateBlockSprite;
            renderer.color = rotateBlockColor;
        }

        public void UnsetAsPivot() {
            renderer.sprite = defaultSprite;
            renderer.color = defaultColor;
        }
    }
}