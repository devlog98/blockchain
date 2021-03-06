using devlog98.Audio;
using devlog98.Block;
using devlog98.InputSystem;
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
        [SerializeField] private PlayerMove playerMove;

        [Header("Rotation")]
        [SerializeField] private PlayerRotate playerRotate;

        [Header("Feedback")]
        [SerializeField] private List<ParticleSystem> explosionParticles;
        
        [SerializeField] private AudioClip spareClip;
        [SerializeField] private AudioClip spikeClip;
        [SerializeField] private AudioClip levelCompletedClip;        

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

            playerMove.ExecuteStart();
        }

        private void Update() {
            if (isAlive) {
                //grabs input
                InputManager.GetInput();
                float horizontalInput = InputManager.GetHorizontal();
                float verticalInput = InputManager.GetVertical();

                if (!playerRotate.IsPivoting) {
                    playerMove.ExecuteUpdate(horizontalInput, verticalInput);
                }
                else {
                    if (!playerMove.IsMoving) {
                        playerRotate.ExecuteUpdate(horizontalInput);
                    }
                }
            }
            
            if (canReset) {
                // reload level
                if (InputManager.GetRestartDown()) {
                    GM.GM.instance.ReloadScene();
                    isAlive = false;
                }

                // return to menu
                if (InputManager.GetExitDown()) {
                    GM.GM.instance.LoadScene(0);
                    isAlive = false;
                }
            }
        }

        // move Player
        private void FixedUpdate() {
            playerMove.ExecuteFixedUpdate();
            playerRotate.ExecuteFixedUpdate();
        }        

        // change move direction for actions other than moving
        public void UpdateDirection(PlayerDirection newDirection) {
            playerMove.MoveDirection = newDirection;
        }

        // prepare rotation based on specific block
        public void StartRotation(PlayerBlock block, Color color) {
            playerRotate.ExecuteStart(block, color);
            ActivateExplosions(block.transform.position, color);
        }

        // finish rotation and recalculate blocks positions
        public void EndRotation(PlayerBlock block, Color color) {
            ActivateExplosions(block.transform.position, color);
            CheckBlockNeighbours();
        }

        // add specific block
        public void AddBlock(PlayerBlock block, PlayerBlock neighbourBlock, Color color) {
            Vector3 blockSpacing = Vector3.zero;
            if (!playerRotate.IsRotating) {
                switch (playerMove.MoveDirection) {
                    case PlayerDirection.Right:
                        blockSpacing = Vector3.right * playerMove.MoveDistance;
                        break;
                    case PlayerDirection.Left:
                        blockSpacing = Vector3.left * playerMove.MoveDistance;
                        break;
                    case PlayerDirection.Up:
                        blockSpacing = Vector3.up * playerMove.MoveDistance;
                        break;
                    case PlayerDirection.Down:
                        blockSpacing = Vector3.down * playerMove.MoveDistance;
                        break;
                }
            }
            else {
                float distanceX = (neighbourBlock.transform.position.x - block.transform.position.x);
                float distanceY = (neighbourBlock.transform.position.y - block.transform.position.y);

                if (distanceX < -0.64f) {
                    // x -> -1
                    if (distanceY > 0.64f && playerMove.MoveDirection == PlayerDirection.Left) {
                        // y -> 1 down
                        blockSpacing = neighbourBlock.transform.up * -1 * playerMove.MoveDistance;
                    }
                    else if (distanceY < -0.64f && playerMove.MoveDirection == PlayerDirection.Right) {
                        // y -> -1 up
                        blockSpacing = neighbourBlock.transform.up * playerMove.MoveDistance;
                    }
                    else {
                        // y -> 0
                        blockSpacing = neighbourBlock.transform.right * playerMove.MoveDistance;
                    }
                }
                else if (distanceX > 0.64f) {
                    // x -> 1
                    if (distanceY > 0.64f && playerMove.MoveDirection == PlayerDirection.Right) {
                        // y -> 1 down
                        blockSpacing = neighbourBlock.transform.up * -1 * playerMove.MoveDistance;
                    }
                    else if (distanceY < -0.64f && playerMove.MoveDirection == PlayerDirection.Left) {
                        // y -> -1 up
                        blockSpacing = neighbourBlock.transform.up * playerMove.MoveDistance;
                    }
                    else {
                        // y -> 0
                        blockSpacing = neighbourBlock.transform.right * -1 * playerMove.MoveDistance;
                    }
                }
                else {
                    // x -> 0
                    if (distanceY > 0.64f) {
                        // y -> 1 down
                        blockSpacing = neighbourBlock.transform.up * -1 * playerMove.MoveDistance;
                    }
                    else if (distanceY < -0.64f) {
                        // y -> -1 up
                        blockSpacing = neighbourBlock.transform.up * playerMove.MoveDistance;
                    }
                }
            }

            block.gameObject.name += " (" + (blocks.Count + 1) + ")";
            block.transform.position = neighbourBlock.transform.position + blockSpacing;
            block.transform.rotation = neighbourBlock.transform.rotation;
            block.transform.parent = (playerRotate.IsRotating) ? playerRotate.Pivot : this.transform;

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

        // level completed
        public void LevelCompleted() {
            isAlive = false;
            AudioManager.instance.PlayOneShot(levelCompletedClip);
            foreach (PlayerBlock block in blocks) {
                ActivateExplosions(block.transform.position, block.LevelCompletedColor, true);
                block.SetAsLevelCompleted();
            }
        }

        // check all blocks neighbours
        private void CheckBlockNeighbours() {
            foreach (PlayerBlock block in blocks) {
                block.CheckBlockNeighbours();
            }
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
    }
}