using UnityEngine;

/*
 * General manager for Player control
 */

namespace devlog98.InputSystem {
    public static class InputManager {
        // gets all inputs from system
        public static void GetInput() {
            SwipeManager.GetSwipe();
        }

        public static float GetHorizontal() {
            if (SwipeManager.SwipeLeft || SwipeManager.SwipeRight) {
                return SwipeManager.SwipeLeft == true ? -1 : 1;
            }
            else {
                return Input.GetAxisRaw("Horizontal");
            }
        }

        public static float GetVertical() {
            if (SwipeManager.SwipeDown || SwipeManager.SwipeUp) {
                return SwipeManager.SwipeDown == true ? -1 : 1;
            }
            else {
                return Input.GetAxisRaw("Vertical");
            }
        }

        public static bool GetHorizontalDown() {
            return SwipeManager.SwipeLeft || SwipeManager.SwipeRight || Input.GetButtonDown("Horizontal");
        }

        public static bool GetVerticalDown() {
            return SwipeManager.SwipeDown || SwipeManager.SwipeUp || Input.GetButtonDown("Vertical");
        }

        public static bool GetRestartDown() {
            return Input.GetKeyDown(KeyCode.R);
        }

        public static bool GetExitDown() {
            return Input.GetKeyDown(KeyCode.E);
        }
    }
}