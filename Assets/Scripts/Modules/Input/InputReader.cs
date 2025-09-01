using UnityEngine;
using UnityEngine.InputSystem;
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

        // Input Actions asset (auto-generated class)
        public InputActions inputActions;

        // Event callbacks
        public event Action<int> horizontalEvent;
        public event Action<int> verticalEvent;
        public event Action confirmEvent;
        public event Action backEvent;

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
            if (context.performed)
            {
                int horizontalValue = (int)context.ReadValue<float>();
                horizontalEvent?.Invoke(horizontalValue);
            }
        }

        public void OnVertical(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                int verticalValue = (int)context.ReadValue<float>();
                verticalEvent?.Invoke(verticalValue);
            }
        }
    }
}