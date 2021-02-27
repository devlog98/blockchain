using devlog98.Block;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Object that Player will be able to control on the game
 */

namespace devlog98.Actor {
    public enum PlayerDirection { Right, Left, Up, Down } // all possible move directions

    public class Player : MonoBehaviour {
        private List<PlayerBlock> blocks = new List<PlayerBlock>();

        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float moveDistance;
        [SerializeField] private float moveSpeed;
        public Vector3 targetPosition;
        public bool isMoving;
        public PlayerDirection moveDirection;

        [Header("Collision")]
        [SerializeField] private float collisionDistance;
        [SerializeField] private float collisionRaySpacing;
        [SerializeField] private LayerMask blockMask;
        [SerializeField] private LayerMask wallMask;

        private void Start() {
            blocks.AddRange(GetComponentsInChildren<PlayerBlock>());
            foreach (PlayerBlock block in blocks) {
                block.ExecuteStart(collisionDistance, collisionRaySpacing, blockMask);
            }

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
                    checkBlocks = blocks.FindAll(x => x.RightBlock == null);
                    break;
                case PlayerDirection.Left:
                    checkBlocks = blocks.FindAll(x => x.LeftBlock == null);
                    break;
                case PlayerDirection.Up:
                    checkBlocks = blocks.FindAll(x => x.UpBlock == null);
                    break;
                case PlayerDirection.Down:
                    checkBlocks = blocks.FindAll(x => x.DownBlock == null);
                    break;
            }

            foreach(PlayerBlock checkBlock in checkBlocks) {                
                GameObject wall = checkBlock.CheckBlockOnDirection(moveDirection, collisionDistance, collisionRaySpacing, wallMask);
                if (wall != null) {
                    check = true;
                    break;
                }
            }

            return check;
        }
    }
}