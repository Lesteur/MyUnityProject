using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

namespace Game.Input
{
    /// <summary>
    /// Singleton class responsible for reading player input and broadcasting input events.
    /// Uses the Unity Input System and a custom PlayerInputActions asset.
    /// </summary>
    public class InputReader : MonoBehaviour, InputActions.IGlobalActions
    {
        // Singleton instance
        public static InputReader Instance { get; private set; }

        public float moveRepeatDelay = 0.5f;
        public float moveRepeatRate = 0.1f;

        // Input Actions asset (auto-generated class)
        public InputActions inputActions;

        // Event callbacks
        public event Action<int> horizontalEvent;
        public event Action<int> verticalEvent;
        public event Action confirmEvent;
        public event Action backEvent;

        private Coroutine moveCoroutine;

        private void Awake()
        {
            // Ensure single instance
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Optional: persist across scenes

            inputActions = new InputActions();
            inputActions.Global.SetCallbacks(this);
        }

        private void OnEnable()
        {
            Debug.Log("InputReader enabled.");

            inputActions.Global.Enable();
        }

        private void OnDisable()
        {
            inputActions.Global.Disable();
        }

        // --- Actions interface methods ---

        public void OnConfirm(InputAction.CallbackContext context)
        {
            if (context.started)
                confirmEvent?.Invoke();
        }

        public void OnBack(InputAction.CallbackContext context)
        {
            if (context.started)
                backEvent?.Invoke();
        }

        public void OnHorizontal(InputAction.CallbackContext context)
        {
            int horizontalValue = (int)context.ReadValue<float>();

            if (context.performed)
            {
                if (horizontalValue != 0)
                {
                    horizontalEvent?.Invoke(horizontalValue);

                    if (moveCoroutine != null)
                        StopCoroutine(moveCoroutine);
                }

                moveCoroutine = StartCoroutine(RepeatMove(horizontalValue, 0));
            }
            else if (context.canceled)
            {
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                    moveCoroutine = null;
                }
            }
        }

        public void OnVertical(InputAction.CallbackContext context)
        {
            int verticalValue = (int)context.ReadValue<float>();

            if (context.performed)
            {
                if (verticalValue != 0)
                {
                    verticalEvent?.Invoke(verticalValue);

                    if (moveCoroutine != null)
                        StopCoroutine(moveCoroutine);
                }

                moveCoroutine = StartCoroutine(RepeatMove(0, verticalValue));
            }
            else if (context.canceled)
            {
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                    moveCoroutine = null;
                }
            }
        }

        private IEnumerator RepeatMove(int horizontal, int vertical)
        {
            yield return new WaitForSeconds(moveRepeatDelay);

            while (horizontal != 0 || vertical != 0)
            {
                if (horizontal != 0)
                    horizontalEvent?.Invoke(horizontal);
                
                if (vertical != 0)
                    verticalEvent?.Invoke(vertical);

                yield return new WaitForSeconds(moveRepeatRate);
            }
        }
    }
}