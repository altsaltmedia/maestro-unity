using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AltSalt.Maestro.Sensors {

    public class TouchInputManager : MonoBehaviour
    {
        private PlayerInput playerInput;

        public event Action<Gesture> TouchStart;
        public event Action<Gesture> TouchDown;
        public event Action<Gesture> TouchUp;
        public event Action<Gesture> Swipe;
        public event Action<Gesture> SwipeEnd;

        public event Action<Vector2> Scroll;

        private bool gestureStarted = false;
        private Vector2 startPosition;
        private Vector2 previousPosition;
        private bool isDragging = false;
        private float gestureStartTime;
        private float previousTime;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            playerInput.actions["PointerInputStart"].performed += OnPointerInputAction;
            playerInput.actions["ScrollInput"].performed += OnScrollAction;
        }

        protected void OnPointerInputAction(InputAction.CallbackContext context)
        {
            var control = context.control;
            var device = control.device;

            var isMouseInput = device is Mouse;
            var isPenInput = !isMouseInput && device is Pen;

            var pointerInput = context.ReadValue<PointerInput>();

            float currentTime = Time.time;

            if (pointerInput.Contact)
            {

                if (gestureStarted == false)
                {
                    gestureStartTime = Time.time;
                    startPosition = pointerInput.Position;
                    gestureStarted = true;
                    TouchStart?.Invoke(new Gesture { position = pointerInput.Position });
                }

                //TouchDown?.Invoke(new Gesture { position = pointerInput.Position, actionTime = currentTime - gestureStartTime });

                if (isDragging == false && startPosition.Equals(pointerInput.Position) == false)
                {
                    isDragging = true;
                }

                if (isDragging)
                {
                    Swipe?.Invoke(
                        new Gesture
                        {
                            position = pointerInput.Position,
                            deltaPosition = pointerInput.Position - previousPosition,
                            actionTime = currentTime - gestureStartTime,
                            deltaTime = currentTime - previousTime,
                        }
                    );
                }

            }
            else
            {
                TouchUp?.Invoke(new Gesture { });

                if (isDragging)
                {
                    SwipeEnd?.Invoke(
                        new Gesture
                        {
                            position = pointerInput.Position,
                            deltaPosition = pointerInput.Position - previousPosition,
                            actionTime = currentTime - gestureStartTime,
                            deltaTime = currentTime - previousTime,
                        }
                    );
                }
                gestureStarted = false;
                isDragging = false;
            }
            previousTime = currentTime;
            previousPosition = pointerInput.Position;
        }

        protected void OnScrollAction(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            Scroll?.Invoke(value);
        }
    }
}