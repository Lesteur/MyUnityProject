using UnityEngine;
using UnityEngine.InputSystem;

namespace Metroidvania.Core
{
    public abstract class PlayableCharacterBase : MonoBehaviour
    {
        // --- Core Components ---
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Collider2D _col;
        private PlayerInput _playerInput;

        // --- State Machine ---
        private CharacterStateMachine _stateMachine;

        // --- Movement Variables ---
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 10f;
        private Vector2 _moveInput;

        // --- Ground Check ---
        [Header("Ground Detection")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.1f;
        [SerializeField] private LayerMask _groundLayer;
        private bool _isGrounded;

        // --- Facing Direction ---
        private bool _isFacingRight = true;

        // --- Jump Input ---
        private bool _jumpRequested;

        // --- Properties ---
        public Rigidbody2D Rb => _rb;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Animator Animator => _animator;
        public Collider2D Col => _col;
        public PlayerInput PlayerInput => _playerInput;
        public CharacterStateMachine StateMachine => _stateMachine;

        public float MoveSpeed => _moveSpeed;
        public float JumpForce => _jumpForce;
        public Vector2 MoveInput => _moveInput;
        public bool IsGrounded => _isGrounded;
        public bool IsFacingRight => _isFacingRight;
        public bool JumpRequested => _jumpRequested;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _col = GetComponent<Collider2D>();
            _playerInput = GetComponent<PlayerInput>();

            _stateMachine = new CharacterStateMachine(this);
        }

        protected virtual void Start()
        {
            // _stateMachine.Initialize(_stateMachine.IdleState);
        }

        protected virtual void Update()
        {
            UpdateGroundCheck();
            _stateMachine.CurrentState?.LogicUpdate();
            _jumpRequested = false; // Reset jump request each frame
        }

        protected virtual void FixedUpdate()
        {
            _stateMachine.CurrentState?.PhysicsUpdate();
        }

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                _jumpRequested = true;
            }
        }

        private bool CheckGrounded()
        {
            return Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
        }

        private void UpdateGroundCheck()
        {
            _isGrounded = CheckGrounded();
        }

        protected void FlipSpriteIfNeeded(float direction)
        {
            if ((direction > 0 && !_isFacingRight) || (direction < 0 && _isFacingRight))
            {
                _isFacingRight = !_isFacingRight;
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (_groundCheck != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
            }
        }
    }
}