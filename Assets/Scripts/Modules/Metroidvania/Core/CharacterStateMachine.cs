using Metroidvania.Core.States;

namespace Metroidvania.Core
{
    public class CharacterStateMachine
    {
        // --- Fields ---
        private readonly PlayableCharacterBase _character;
        private CharacterState _currentState;
        private readonly CharacterStateIdle _idleState;
        private readonly CharacterStateMove _moveState;
        private readonly CharacterStateJump _jumpState;

        // --- Properties ---
        public PlayableCharacterBase Character => _character;
        public CharacterState CurrentState => _currentState;
        public CharacterStateIdle IdleState => _idleState;
        public CharacterStateMove MoveState => _moveState;
        public CharacterStateJump JumpState => _jumpState;

        // --- Constructor ---
        public CharacterStateMachine(PlayableCharacterBase character)
        {
            _character = character;

            // Initialisation des états principaux
            _idleState = new CharacterStateIdle(this);
            _moveState = new CharacterStateMove(this);
            _jumpState = new CharacterStateJump(this);

            EnterDefaultState();
        }

        /// <summary>
        /// Effectue la transition vers un nouvel état.
        /// </summary>
        /// <param name="newState">Nouvel état à activer.</param>
        public void EnterState(CharacterState newState)
        {
            var previousState = _currentState;
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(previousState);
        }

        /// <summary>
        /// Active l'état par défaut (Idle).
        /// </summary>
        public void EnterDefaultState()
        {
            EnterState(_idleState);
        }
    }
}