namespace Metroidvania.Core.States
{
    /// <summary>
    /// Abstract base class for character states in the state machine.
    /// Provides encapsulated access to the character and state machine.
    /// </summary>
    public abstract class CharacterState
    {
        // --- Fields ---
        protected readonly PlayableCharacterBase Character;
        protected readonly CharacterStateMachine StateMachine;

        /// <summary>
        /// Constructor for a character state.
        /// </summary>
        /// <param name="stateMachine">Reference to the state machine.</param>
        protected CharacterState(CharacterStateMachine stateMachine)
        {
            Character = stateMachine.Character;
            StateMachine = stateMachine;
        }

        /// <summary>
        /// Called when entering the state.
        /// </summary>
        /// <param name="previousState">The previous state.</param>
        public virtual void Enter(CharacterState previousState) { }

        /// <summary>
        /// Called every frame for logic updates.
        /// </summary>
        public virtual void LogicUpdate() { }

        /// <summary>
        /// Called every physics frame for physics updates.
        /// </summary>
        public virtual void PhysicsUpdate() { }

        /// <summary>
        /// Called when exiting the state.
        /// </summary>
        public virtual void Exit() { }
    }
}