using UnityEngine;

namespace TacticalRPG
{
    /// <summary>
    /// Base class for tactical gameplay states.  
    /// Provides virtual methods for handling lifecycle events, input, updates, and rendering.  
    /// </summary>
    public abstract class TacticalStateBase
    {
        /// <summary>
        /// Reference to the owning state machine.
        /// </summary>
        protected readonly TacticalStateMachine stateMachine;

        /// <summary>
        /// Shortcut to the tactical controller managed by the state machine.
        /// </summary>
        protected TacticalController Controller => stateMachine.Controller;

        /// <summary>
        /// Creates a new tactical state bound to a state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine managing this state.</param>
        protected TacticalStateBase(TacticalStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        /// <summary>
        /// Called when entering this state.
        /// </summary>
        /// <param name="previousState">The previously active state.</param>
        public virtual void Enter(TacticalStateBase previousState) { }

        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        /// Called once per frame to update state logic.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Called in FixedUpdate to handle physics-related logic.
        /// </summary>
        public virtual void PhysicsUpdate() { }

        /// <summary>
        /// Called after Update to check for transitions between states.
        /// </summary>
        public virtual void Transition() { }

        /// <summary>
        /// Called to update visual rendering of the state (e.g., highlights).
        /// </summary>
        public virtual void UpdateRendering() { }

        /// <summary>
        /// Handles horizontal input.
        /// </summary>
        /// <param name="direction">-1 for left, +1 for right.</param>
        public virtual void HorizontalKey(int direction) { }

        /// <summary>
        /// Handles vertical input.
        /// </summary>
        /// <param name="direction">-1 for down, +1 for up.</param>
        public virtual void VerticalKey(int direction) { }

        /// <summary>
        /// Handles confirm/accept input.
        /// </summary>
        public virtual void ConfirmKey() { }

        /// <summary>
        /// Handles cancel/back input.
        /// </summary>
        public virtual void CancelKey() { }

        /// <summary>
        /// Handles tactical menu button click events.
        /// </summary>
        /// <param name="buttonIndex">The index of the clicked button.</param>
        public virtual void OnClickButton(int buttonIndex) { }

        /// <summary>
        /// Handles tile click events.
        /// </summary>
        public virtual void OnTileClicked(Tile tile) { }

        /// <summary>
        /// Gets the tile currently under the mouse cursor.
        /// </summary>
        public virtual void OnTileHovered(Tile tile) { }

        /// <summary>
        /// Called when the mouse cursor exits a tile.
        /// </summary>
        public virtual void OnTileHoverExited() { }
    }
}