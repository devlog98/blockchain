using devlog98.Audio;
using devlog98.Block;
using UnityEngine;

/*
 * Responsible for Player rotation
 * Sets and unsets pivots
 */

namespace devlog98.Actor {
    public class PlayerRotate : MonoBehaviour {
        [Header("Rotation")]
        [SerializeField] private Rigidbody2D rbPivot;
        [SerializeField] private Transform pivot;
        [SerializeField] private float rotateAmount;
        [SerializeField] private float rotateSpeed;
        private PlayerBlock pivotBlock;
        private Vector3 targetRotation;
        private bool isPivoting;
        private bool isRotating;
        private const float rotationThreshold = 0.1f;

        public Transform Pivot { get => pivot; }
        public bool IsPivoting { get => isPivoting; }        
        public bool IsRotating { get => isRotating; }

        [Header("Feedback")]
        [SerializeField] private AudioClip walkClip;
        [SerializeField] private AudioClip rotateClip;

        // initialize variables
        public void ExecuteStart(PlayerBlock block, Color color) {
            isPivoting = true;
            pivotBlock = block;
            pivotBlock.SetAsPivot();

            AudioManager.instance.PlayOneShot(rotateClip);
        }

        // set rotation based on input
        public void ExecuteUpdate(float horizontalInput) {
            // change pivot position once
            if (pivot.transform.position != pivotBlock.transform.position) {
                SetPivotOnBlock();
            }
            
            if (!isRotating) {
                // default rotation
                if (horizontalInput != 0) {
                    RotateOnHorizontalAxis(horizontalInput);
                }
            }
        }

        // rotate to angle
        public void ExecuteFixedUpdate() {
            if (isPivoting) {
                // compare to rotation threshold
                float currentThreshold = Quaternion.Angle(pivot.rotation, Quaternion.Euler(targetRotation.x, targetRotation.y, targetRotation.z));
                if (currentThreshold > rotationThreshold) {
                    // interpolates current rotation with target rotation
                    float moveRotation = Mathf.MoveTowardsAngle(pivot.eulerAngles.z, targetRotation.z, rotateSpeed * Time.deltaTime);
                    rbPivot.MoveRotation(moveRotation);                    
                }
                else if (isRotating) {
                    // end rotation
                    isRotating = false;
                    UnsetPivotFromBlock();
                }
            }
        }

        // set next rotation
        private void RotateOnHorizontalAxis(float horizontalInput) {
            PlayerDirection newDirection;
            if (horizontalInput > 0) {
                targetRotation = new Vector3(0, 0, pivot.eulerAngles.z - rotateAmount);
                newDirection = PlayerDirection.Right;
            }
            else {
                targetRotation = new Vector3(0, 0, pivot.eulerAngles.z + rotateAmount);
                newDirection = PlayerDirection.Left;
            }

            isRotating = true;
            AudioManager.instance.PlayOneShot(walkClip);
            Player.instance.UpdateDirection(newDirection);
        }

        // start rotation based on specific block
        private void SetPivotOnBlock() {
            pivot.position = pivotBlock.transform.position;
            foreach (PlayerBlock block in Player.instance.Blocks) {
                block.transform.parent = pivot;
            }
        }

        // stop rotation based on specific block
        private void UnsetPivotFromBlock() {
            // restore blocks rotation
            foreach (PlayerBlock block in Player.instance.Blocks) {
                block.transform.parent = this.transform;
                block.transform.eulerAngles = Vector3.zero;
            }

            // end rotation
            isPivoting = false;
            pivotBlock.UnsetAsPivot();
            AudioManager.instance.PlayOneShot(rotateClip);
            Player.instance.EndRotation(pivotBlock, pivotBlock.RotateBlockColor);
            pivotBlock = null;            
        }
    }
}