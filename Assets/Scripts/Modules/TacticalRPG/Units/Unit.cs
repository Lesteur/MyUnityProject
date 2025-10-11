using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TacticalRPG.Core;
using TacticalRPG.Paths;
using TacticalRPG.Skills;

namespace TacticalRPG.Units
{
    /// <summary>
    /// Represents a tactical unit capable of moving, acting, and using skills on the grid.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Unit : MonoBehaviour
    {
        public enum UnitType { Player, Enemy, Neutral }

        [Header("Unit Settings")]
        [SerializeField] private Vector2Int _startPosition;
        [SerializeField] private int _movementPoints = 5;
        [SerializeField] private int _jumpHeight = 1;
        [SerializeField] private int _maxFallHeight = 10;
        [SerializeField] private UnitType _unitType = UnitType.Player;
        [SerializeField] private List<SkillData> _skills = new();

        private Tile _currentTile;
        private Tile _previousTile;
        private SpriteRenderer _spriteRenderer;
        private PathResult _currentPath;
        private Coroutine _movementCoroutine;

        private readonly Dictionary<SkillData, List<Vector2Int>> _movementPatterns = new();

        #region Events

        /// <summary>
        /// Fired when the unit finishes its movement animation and reaches destination.
        /// </summary>
        public event System.Action<Unit> OnMovementComplete;

        // public event System.Action<Unit> OnActionComplete;

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets whether the unit has ended its turn.
        /// </summary>
        public bool EndTurn { get; set; }
        /// <summary>
        /// Gets or sets whether the unit has completed its movement.
        /// </summary>
        public bool MovementDone { get; set; }
        /// <summary>
        /// Gets or sets whether the unit has completed its action.
        /// </summary>
        public bool ActionDone { get; set; }

        /// <summary>
        /// Gets or sets the available paths for this unit.
        /// </summary>
        public List<PathResult> AvailablePaths { get; private set; } = new();
        /// <summary>
        /// Gets the current tile occupied by this unit.
        /// </summary>
        public Tile CurrentTile => _currentTile;
        /// <summary>
        /// Gets the previous tile occupied by this unit.
        /// </summary>
        public Tile PreviousTile => _previousTile;
        /// <summary>
        /// Gets the grid position of this unit.
        /// </summary>
        public Vector2Int GridPosition => _currentTile != null ? _currentTile.GridPosition : _startPosition;
        /// <summary>
        /// Gets the movement points of this unit.
        /// </summary>
        public int MovementPoints => _movementPoints;
        /// <summary>
        /// Gets the jump height of this unit.
        /// </summary>
        public int JumpHeight => _jumpHeight;
        /// <summary>
        /// Gets the maximum fall height of this unit.
        /// </summary>
        public int MaxFallHeight => _maxFallHeight;
        /// <summary>
        /// Gets the type of this unit.
        /// </summary>
        public UnitType Type => _unitType;
        /// <summary>
        /// Gets the list of skills for this unit.
        /// </summary>
        public List<SkillData> Skills => _skills;
        /// <summary>
        /// Gets the movement patterns for each skill.
        /// </summary>
        public Dictionary<SkillData, List<Vector2Int>> MovementPatterns => _movementPatterns;

        #endregion
        
        #region Initialization

        /// <summary>
        /// Unity Awake callback. Initializes the sprite renderer.
        /// </summary>
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Initializes the unit's position and movement patterns.
        /// </summary>
        public void Initialize()
        {
            var tile = TacticalController.Instance.GetTileAt(_startPosition);
            if (tile == null)
            {
                Debug.LogError($"Unit '{name}' could not find starting tile at {_startPosition}.");
                return;
            }

            _currentTile = tile;
            _currentTile.OccupyingUnit = this;
            _previousTile = null;

            MoveToTile(_currentTile);

            foreach (var skill in _skills)
            {
                if (skill != null)
                    _movementPatterns[skill] = skill.AreaOfEffect.GetAllRangedPositions();
            }
        }

        #endregion
        
        #region Movement

        /// <summary>
        /// Starts moving the unit along the given path using coroutine animation.
        /// </summary>
        /// <param name="pathResult">The path to follow.</param>
        public void FollowPath(PathResult pathResult)
        {
            if (!pathResult.IsValid)
            {
                Debug.LogError($"Invalid path for unit '{name}'.");
                return;
            }

            _currentPath = pathResult;

            if (_movementCoroutine != null)
                StopCoroutine(_movementCoroutine);

            _movementCoroutine = StartCoroutine(MoveAlongPath());
        }

        /// <summary>
        /// Coroutine for moving the unit along its path.
        /// </summary>
        private IEnumerator MoveAlongPath()
        {
            foreach (var tile in _currentPath.Path)
            {
                Vector3 targetPos = new(tile.transform.position.x, tile.transform.position.y + 0.3f, 0);

                while (Vector3.Distance(transform.position, targetPos) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 3.5f);
                    yield return null;
                }

                transform.position = targetPos;
                _spriteRenderer.sortingOrder = tile.Order + 2;
            }

            SetPosition(_currentPath.Path[^1]);
            
            OnMovementComplete?.Invoke(this);
        }

        /// <summary>
        /// Instantly moves the unit to the specified tile.
        /// </summary>
        /// <param name="tile">The tile to move to.</param>
        private void MoveToTile(Tile tile)
        {
            if (tile == null) return;

            Vector3 pos = tile.transform.position;
            transform.position = new Vector3(pos.x, pos.y + 0.3f, 0);
            _spriteRenderer.sortingOrder = tile.Order + 2;
        }

        #endregion
        
        #region Position & Skills

        /// <summary>
        /// Sets the available paths for this unit.
        /// </summary>
        /// <param name="paths">The list of available paths.</param>
        public void SetAvailablePaths(List<PathResult> paths)
            => AvailablePaths = (paths != null && paths.Count > 0) ? paths : new();

        /// <summary>
        /// Gets the skill at the specified index.
        /// </summary>
        /// <param name="index">The index of the skill.</param>
        /// <returns>The skill at the index, or null if out of range.</returns>
        public SkillData GetSkillByIndex(int index)
            => (_skills != null && index >= 0 && index < _skills.Count) ? _skills[index] : null;

        /// <summary>
        /// Sets the unit's position to the specified tile.
        /// </summary>
        /// <param name="newTile">The tile to move to.</param>
        public void SetPosition(Tile newTile)
        {
            if (newTile == null) return;

            _previousTile = _currentTile;

            if (_currentTile != null)
                _currentTile.OccupyingUnit = null;

            _currentTile = newTile;
            _currentTile.OccupyingUnit = this;

            MoveToTile(_currentTile);
        }

        /// <summary>
        /// Sets the unit's position to the specified grid position.
        /// </summary>
        /// <param name="newPosition">The grid position to move to.</param>
        public void SetPosition(Vector2Int newPosition)
        {
            var tile = TacticalController.Instance.GetTileAt(newPosition);
            SetPosition(tile);
        }

        #endregion
    }
}