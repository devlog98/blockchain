using devlog98.Audio;
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

        [Header("General")]
        [SerializeField] private bool canReset = true;
        private List<PlayerBlock> blocks = new List<PlayerBlock>();
        public List<PlayerBlock> Blocks { get => blocks; }
        private bool isAlive = true;

        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float moveDistance;
        [SerializeField] private float moveSpeed;
        public Vector3 targetPosition;
        public bool isMoving;
        public PlayerDirection moveDirection;

        [Header("Rotation")]
        [SerializeField] private Rigidbody2D rbPivot;
        [SerializeField] private Transform pivot;
        [SerializeField] private float rotateAmount;
        [SerializeField] private float rotateSpeed;
        private PlayerBlock pivotBlock;
        private Vector3 targetRotation;
        public bool isPivoting;
        public bool isRotating;

        [Header("Feedback")]
        [SerializeField] private List<ParticleSystem> explosionParticles;
        [SerializeField] private AudioClip walkClip;
        [SerializeField] private AudioClip spareClip;
        [SerializeField] private AudioClip spikeClip;
        [SerializeField] private AudioClip rotateClip;
        [SerializeField] private AudioClip levelCompletedClip;
        [SerializeField] private AudioClip selectClip;

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
            if (isAlive) {
                //grabs input
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                float verticalInput = Input.GetAxisRaw("Vertical");

                if (!isPivoting) {
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
                else {
                    if (!isMoving) {
                        if (pivot.transform.position != pivotBlock.transform.position) {
                            SetPivotOnBlock();
                        }

                        if (!isRotating) {
                            // rotate horizontally
                            if (horizontalInput != 0) {
                                RotateOnHorizontalAxis(horizontalInput);
                            }
                        }
                    }
                }
            }
            
            if (canReset) {
                // reload level
                if (Input.GetKeyDown(KeyCode.R)) {
                    GM.GM.instance.ReloadScene();
                    AudioManager.instance.PlayOneShot(selectClip);
                    isAlive = false;
                }

                // return to menu
                if (Input.GetKeyDown(KeyCode.E)) {
                    GM.GM.instance.LoadScene(0);
                    AudioManager.instance.PlayOneShot(selectClip);
                    isAlive = false;
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
            }
            else if (isMoving) {
                isMoving = false;
            }

            if (isPivoting) {
                if (Quaternion.Angle(pivot.rotation, Quaternion.Euler(targetRotation.x, targetRotation.y, targetRotation.z)) > 0.1f) {
                    float moveRotation = Mathf.MoveTowardsAngle(pivot.eulerAngles.z, targetRotation.z, rotateSpeed * Time.deltaTime);

                    rbPivot.MoveRotation(moveRotation);
                    isRotating = true;
                }
                else if (isRotating) {
                    isRotating = false;
                    UnsetPivotFromBlock();
                }
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
            AudioManager.instance.PlayOneShot(walkClip);
            isMoving = true;
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
            AudioManager.instance.PlayOneShot(walkClip);
            isMoving = true;
        }

        // set next rotation
        private void RotateOnHorizontalAxis(float horizontalInput) {
            if (horizontalInput > 0) {
                targetRotation = new Vector3(0, 0, pivot.eulerAngles.z - rotateAmount);
                moveDirection = PlayerDirection.Right;
            }
            else {
                targetRotation = new Vector3(0, 0, pivot.eulerAngles.z + rotateAmount);
                moveDirection = PlayerDirection.Left;
            }
            AudioManager.instance.PlayOneShot(walkClip);
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
                GameObject wall = checkBlock.CheckCollisionOnDirection(moveDirection, false);
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
        public void AddBlock(PlayerBlock block, PlayerBlock neighbourBlock, Color color) {
            Vector3 blockSpacing = Vector3.zero;
            if (!isRotating) {
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
            }
            else {
                float distanceX = (neighbourBlock.transform.position.x - block.transform.position.x);
                float distanceY = (neighbourBlock.transform.position.y - block.transform.position.y);

                if (distanceX < -0.64f) {
                    // x -> -1
                    if (distanceY > 0.64f && moveDirection == PlayerDirection.Left) {
                        // y -> 1 down
                        blockSpacing = neighbourBlock.transform.up * -1 * moveDistance;
                    }
                    else if (distanceY < -0.64f && moveDirection == PlayerDirection.Right) {
                        // y -> -1 up
                        blockSpacing = neighbourBlock.transform.up * moveDistance;
                    }
                    else {
                        // y -> 0
                        blockSpacing = neighbourBlock.transform.right * moveDistance;
                    }
                }
                else if (distanceX > 0.64f) {
                    // x -> 1
                    if (distanceY > 0.64f && moveDirection == PlayerDirection.Right) {
                        // y -> 1 down
                        blockSpacing = neighbourBlock.transform.up * -1 * moveDistance;
                    }
                    else if (distanceY < -0.64f && moveDirection == PlayerDirection.Left) {
                        // y -> -1 up
                        blockSpacing = neighbourBlock.transform.up * moveDistance;
                    }
                    else {
                        // y -> 0
                        blockSpacing = neighbourBlock.transform.right * -1 * moveDistance;
                    }
                }
                else {
                    // x -> 0
                    if (distanceY > 0.64f) {
                        // y -> 1 down
                        blockSpacing = neighbourBlock.transform.up * -1 * moveDistance;
                    }
                    else if (distanceY < -0.64f) {
                        // y -> -1 up
                        blockSpacing = neighbourBlock.transform.up * moveDistance;
                    }
                }
            }

            block.gameObject.name += " (" + (blocks.Count + 1) + ")";
            block.transform.position = neighbourBlock.transform.position + blockSpacing;
            block.transform.rotation = neighbourBlock.transform.rotation;
            block.transform.parent = (isRotating) ? pivot.transform : this.transform;

            AudioManager.instance.PlayOneShot(spareClip);
            ActivateExplosions(block.transform.position, color);
            blocks.Add(block);
            CheckBlockNeighbours();
        }

        // destroy specific block
        public void DestroyBlock(PlayerBlock block, Color color) {
            // remove block
            AudioManager.instance.PlayOneShot(spikeClip);
            ActivateExplosions(block.transform.position, color);
            blocks.Remove(block);
            Destroy(block.gameObject);

            if (blocks.Count > 0) {
                CheckBlockNeighbours();
            }
            else {
                isAlive = false;
            }
        }

        // prepare rotation based on specific block
        public void SetPivotBlock(PlayerBlock block, Color color) {
            pivotBlock = block;
            pivotBlock.SetAsPivot();

            AudioManager.instance.PlayOneShot(rotateClip);
            ActivateExplosions(block.transform.position, color);
            isPivoting = true;
        }

        // start rotation based on specific block
        private void SetPivotOnBlock() {
            pivot.position = pivotBlock.transform.position;
            foreach (PlayerBlock block in blocks) {
                block.transform.parent = pivot;
            }
        }

        // stop rotation based on specific block
        private void UnsetPivotFromBlock() {
            pivotBlock.UnsetAsPivot();
            AudioManager.instance.PlayOneShot(rotateClip);
            ActivateExplosions(pivotBlock.transform.position, pivotBlock.RotateBlockColor);
            pivotBlock = null;

            foreach (PlayerBlock block in blocks) {
                block.transform.parent = this.transform;
                block.transform.eulerAngles = Vector3.zero;
            }

            CheckBlockNeighbours();
            isPivoting = false;
        }

        // play particles with color
        private void ActivateExplosions(Vector3 position, Color color, bool instantiate = false) {
            foreach (ParticleSystem explosionParticle in explosionParticles) {
                explosionParticle.transform.position = position;
                ParticleSystem.MainModule main = explosionParticle.main;
                main.startColor = color;

                if (instantiate) {
                    ParticleSystem newExplosionParticle = Instantiate(
                        explosionParticle.gameObject,
                        explosionParticle.transform.position,
                        explosionParticle.transform.rotation,
                        transform
                    ).GetComponent<ParticleSystem>();

                    newExplosionParticle.Play();
                }
                else {
                    explosionParticle.Play();
                }
            }
        }

        // level completed
        public void LevelCompleted() {
            isAlive = false;
            AudioManager.instance.PlayOneShot(levelCompletedClip);
            foreach (PlayerBlock block in blocks) {
                ActivateExplosions(block.transform.position, block.LevelCompletedColor, true);
                block.SetAsLevelCompleted();
            }
        }
    }
}