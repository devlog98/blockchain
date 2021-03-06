using devlog98.Audio;
using devlog98.Block;
using System.Collections.Generic;
using UnityEngine;

/*
 * Responsible for Player movement
 * Checks collisions with walls
 */

namespace devlog98.Actor {
    public class PlayerMove : MonoBehaviour {
        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float moveDistance;
        [SerializeField] private float moveSpeed;
        private Vector3 targetPosition;
        private bool isMoving;
        private PlayerDirection moveDirection;

        public float MoveDistance { get => moveDistance; }
        public bool IsMoving { get => isMoving; }
        public PlayerDirection MoveDirection { get => moveDirection; set => moveDirection = value; }

        [Header("Feedback")]
        [SerializeField] private AudioClip walkClip;

        // initialize variables
        public void ExecuteStart() {
            targetPosition = transform.position;
        }

        // set movement based on input and collisions
        public void ExecuteUpdate(float horizontalInput, float verticalInput) {
            if (!isMoving) {
                if (horizontalInput != 0 && verticalInput != 0) {
                    // swap directions when there is horizontal and vertical input
                    if (moveDirection == PlayerDirection.Right || moveDirection == PlayerDirection.Left) {
                        MoveOnVerticalAxis(verticalInput);
                    }
                    else {
                        MoveOnHorizontalAxis(horizontalInput);
                    }
                }
                else {
                    // default movement
                    if (horizontalInput != 0) {
                        MoveOnHorizontalAxis(horizontalInput);
                    }

                    if (verticalInput != 0) {
                        MoveOnVerticalAxis(verticalInput);
                    }
                }

                // stop movement when colliding
                if (MoveCollisionCheck()) {
                    targetPosition = transform.position;
                }
            }
        }

        // move to target
        public void ExecuteFixedUpdate() {
            if (transform.position != targetPosition) {
                // interpolates current position with target position
                Vector2 movePosition = new Vector2();
                movePosition.x = Mathf.MoveTowards(transform.position.x, targetPosition.x, moveSpeed * Time.deltaTime);
                movePosition.y = Mathf.MoveTowards(transform.position.y, targetPosition.y, moveSpeed * Time.deltaTime);

                rb.MovePosition(movePosition);
            }
            else if (isMoving) {
                // end movement
                isMoving = false;
            }
        }

        // set horizontal movement
        private void MoveOnHorizontalAxis(float horizontalInput) {
            isMoving = true;
            AudioManager.instance.PlayOneShot(walkClip);

            if (horizontalInput > 0) {
                targetPosition.x = transform.position.x + moveDistance;
                moveDirection = PlayerDirection.Right;
            }
            else {
                targetPosition.x = transform.position.x - moveDistance;
                moveDirection = PlayerDirection.Left;
            }
        }

        // set vertical movement
        private void MoveOnVerticalAxis(float verticalInput) {
            isMoving = true;
            AudioManager.instance.PlayOneShot(walkClip);

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
                    checkBlocks = Player.instance.Blocks.FindAll(x => x != null && x.RightBlock == null);
                    break;
                case PlayerDirection.Left:
                    checkBlocks = Player.instance.Blocks.FindAll(x => x != null && x.LeftBlock == null);
                    break;
                case PlayerDirection.Up:
                    checkBlocks = Player.instance.Blocks.FindAll(x => x != null && x.UpBlock == null);
                    break;
                case PlayerDirection.Down:
                    checkBlocks = Player.instance.Blocks.FindAll(x => x != null && x.DownBlock == null);
                    break;
            }

            // check collisions
            foreach (PlayerBlock checkBlock in checkBlocks) {
                GameObject wall = checkBlock.CheckCollisionOnDirection(moveDirection, false);
                if (wall != null) {
                    check = true;
                    break;
                }
            }

            return check;
        }
    }
}