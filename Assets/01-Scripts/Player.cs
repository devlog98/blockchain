using devlog98.Block;
using System.Collections.Generic;
using UnityEngine;

/*
 * Object that Player will be able to control on the game
 */

namespace devlog98.Actor {
    public enum PlayerDirection { Right, Left, Up, Down } // all possible move directions

    public class Player : MonoBehaviour {
        public static Player instance; // singleton

        public List<PlayerBlock> blocks = new List<PlayerBlock>();
        public List<PlayerBlock> Blocks { get => blocks; }

        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float moveDistance;
        [SerializeField] private float moveSpeed;
        public Vector3 targetPosition;
        public bool isMoving;
        public PlayerDirection moveDirection;

        // initialize singleton
        private void Awake() {
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
            }
            else {
                instance = this;
            }
        }

        private void Start() {
            blocks.AddRange(GetComponentsInChildren<PlayerBlock>());
            CheckBlockNeighbours();

            targetPosition = transform.position;
        }

        private void Update() {
            //grabs input
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (!isMoving) {
                if (horizontalInput != 0 && verticalInput != 0) {
                    // swap between horizontal and vertical directions if Player holds both buttons
                    if (moveDirection == PlayerDirection.Right || moveDirection == PlayerDirection.Left) {
                        MoveOnVerticalAxis(verticalInput);
                    }
                    else {
                        MoveOnHorizontalAxis(horizontalInput);
                    }
                }
                else {
                    // move horizontally or vertically
                    if (horizontalInput != 0) {
                        MoveOnHorizontalAxis(horizontalInput);
                    }

                    if (verticalInput != 0) {
                        MoveOnVerticalAxis(verticalInput);
                    }
                }

                // stop moving if collision is found
                if (MoveCollisionCheck()) {
                    targetPosition = transform.position;
                }
            }
        }

        // move Player
        private void FixedUpdate() {
            if (transform.position != targetPosition) {
                // interpolates current position with target position
                Vector2 movePosition = new Vector2();
                movePosition.x = Mathf.MoveTowards(transform.position.x, targetPosition.x, moveSpeed * Time.deltaTime);
                movePosition.y = Mathf.MoveTowards(transform.position.y, targetPosition.y, moveSpeed * Time.deltaTime);

                rb.MovePosition(movePosition);
                isMoving = true;
            }
            else {
                isMoving = false;
            }
        }

        // set next movement
        private void MoveOnHorizontalAxis(float horizontalInput) {
            if (horizontalInput > 0) {
                targetPosition.x = transform.position.x + moveDistance;
                moveDirection = PlayerDirection.Right;
            }
            else {
                targetPosition.x = transform.position.x - moveDistance;
                moveDirection = PlayerDirection.Left;
            }
        }

        private void MoveOnVerticalAxis(float verticalInput) {
            if (verticalInput > 0) {
                targetPosition.y = transform.position.y + moveDistance;
                moveDirection = PlayerDirection.Up;
            }
            else {
                targetPosition.y = transform.position.y - moveDistance;
                moveDirection = PlayerDirection.Down;
            }
        }

        // check if movement is possible
        private bool MoveCollisionCheck() {
            bool check = false;

            // get blocks that can collide given direction
            List<PlayerBlock> checkBlocks = new List<PlayerBlock>();
            switch (moveDirection) {
                case PlayerDirection.Right:
                    checkBlocks = blocks.FindAll(x => x != null && x.RightBlock == null);
                    break;
                case PlayerDirection.Left:
                    checkBlocks = blocks.FindAll(x => x != null && x.LeftBlock == null);
                    break;
                case PlayerDirection.Up:
                    checkBlocks = blocks.FindAll(x => x != null && x.UpBlock == null);
                    break;
                case PlayerDirection.Down:
                    checkBlocks = blocks.FindAll(x => x != null && x.DownBlock == null);
                    break;
            }

            foreach (PlayerBlock checkBlock in checkBlocks) {
                GameObject wall = checkBlock.CheckBlockOnDirection(moveDirection);
                if (wall != null) {
                    check = true;
                    break;
                }
            }

            return check;
        }

        // check all blocks neighbours
        private void CheckBlockNeighbours() {
            foreach (PlayerBlock block in blocks) {
                block.CheckBlockNeighbours();
            }
        }

        // add specific block
        public void AddBlock(PlayerBlock block, PlayerBlock neighbourBlock) {
            Vector3 blockSpacing = Vector3.zero;
            switch (moveDirection) {
                case PlayerDirection.Right:
                    blockSpacing = Vector3.right * moveDistance;
                    break;
                case PlayerDirection.Left:
                    blockSpacing = Vector3.left * moveDistance;
                    break;
                case PlayerDirection.Up:
                    blockSpacing = Vector3.up * moveDistance;
                    break;
                case PlayerDirection.Down:
                    blockSpacing = Vector3.down * moveDistance;
                    break;
            }

            block.transform.position = neighbourBlock.transform.position + blockSpacing;
            block.transform.parent = this.transform;
            blocks.Add(block);
            block.gameObject.name += " (" + blocks.Count + ")";
            CheckBlockNeighbours();
        }

        // destroy specific block
        public void DestroyBlock(PlayerBlock block) {
            blocks.Remove(block);
            Destroy(block.gameObject);
            CheckBlockNeighbours();
        }
    }
}