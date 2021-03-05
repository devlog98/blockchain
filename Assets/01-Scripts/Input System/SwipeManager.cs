using UnityEngine;

/*
 * Swipe functionalities
 */

namespace devlog98.InputSystem {
    public static class SwipeManager {
        private static bool tap, swipeLeft, swipeRight, swipeDown, swipeUp;
        private static Vector2 startTouch, swipeDelta;
        private static bool isDragging;

        public static bool Tap { get => tap; }
        public static bool SwipeLeft { get => swipeLeft; }
        public static bool SwipeRight { get => swipeRight; }
        public static bool SwipeDown { get => swipeDown; }
        public static bool SwipeUp { get => swipeUp; }

        // check touch every frame
        public static void GetSwipe() {
            tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;
            GetTap();
            CalculateSwipe();
        }

        // get tap on screen
        private static void GetTap() {
            // with mouse
            if (Input.GetMouseButtonDown(0)) {
                tap = true;
                isDragging = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0)) {
                Reset();
            }

            // with touch
            if (Input.touchCount > 0) {
                Touch touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began) {
                    tap = true;
                    isDragging = true;
                    startTouch = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    Reset();
                }
            }
        }

        // calculate swipe direction
        private static void CalculateSwipe() {
            swipeDelta = Vector2.zero;

            // get delta from touch to start point
            if (isDragging) {
                if (Input.touchCount > 0) {
                    swipeDelta = Input.touches[0].position - startTouch;
                }
                else if (Input.GetMouseButton(0)) {
                    swipeDelta = (Vector2)Input.mousePosition - startTouch;
                }
            }

            // check if drag crossed the deadzone
            if (swipeDelta.magnitude > 125) {
                // get direction
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y)) {
                    // left or right
                    if (x < 0) {
                        swipeLeft = true;
                    }
                    else {
                        swipeRight = true;
                    }
                }
                else {
                    // up or down
                    if (y < 0) {
                        swipeDown = true;
                    }
                    else {
                        swipeUp = true;
                    }
                }
            }
        }

        // reset touch positions
        private static void Reset() {
            startTouch = swipeDelta = Vector2.zero;
            isDragging = false;
        }
    }
}