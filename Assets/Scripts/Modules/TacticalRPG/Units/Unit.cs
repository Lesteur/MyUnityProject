using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TacticalRPG.Core;
using TacticalRPG.Paths;
using TacticalRPG.Skills;
using UnityEngine.UIElements;

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
        [SerializeField] private UnitType   _unitType       = UnitType.Player;
        [SerializeField] private Vector2Int _startPosition  = Vector2Int.zero;
        [SerializeField] private int        _level          = 1;
        [SerializeField] private UnitData   _data;

        private Tile    _currentTile;
        private Tile    _previousTile;

        private PathResult  _currentPath;
        private Coroutine   _movementCoroutine;

        private int _healthPoints;
        private readonly Dictionary<SkillData, List<Vector2Int>> _movementPatterns = new();

        private SpriteRenderer _spriteRenderer;

        #region Events

        /// <summary> Fired when the unit finishes its movement animation and reaches destination. </summary>
        public event System.Action<Unit> OnMovementComplete;

        // public event System.Action<Unit> OnActionComplete;

        #endregion
        
        #region Properties

        /// <summary> Gets or sets whether the unit has ended its turn. </summary>
        public bool EndTurn { get; set; }
        /// <summary> Gets or sets whether the unit has completed its movement. </summary>
        public bool MovementDone { get; set; }
        /// <summary> Gets or sets whether the unit has completed its action. </summary>
        public bool ActionDone { get; set; }

        /// <summary> Gets or sets the available paths for this unit. </summary>
        public List<PathResult> AvailablePaths { get; private set; } = new();
        /// <summary>  Gets the current tile occupied by this unit. </summary>
        public Tile CurrentTile => _currentTile;
        /// <summary> Gets the previous tile occupied by this unit. </summary>
        public Tile PreviousTile => _previousTile;
        /// <summary> Gets the type of this unit. </summary>
        public UnitType Type => _unitType;
        /// <summary> Gets the grid position of this unit. </summary>
        public Vector2Int GridPosition => _currentTile != null ? _currentTile.GridPosition : _startPosition;

        /// <summary> Lets the level of this unit. </summary> 
        public int Level => _level;
        /// <summary> Gets the maximum health points of this unit. </summary>
        public int HealthPointsMax => _data.BaseHP + (_data.GainHP * (_level - 1));
        /// <summary> Gets the current health points of this unit. </summary>
        public int HealthPoints => _healthPoints;
        /// <summary> Gets the attack value of this unit. </summary>
        public int Attack => _data.BaseAttack + (_data.GainAttack * (_level - 1));
        /// <summary> Gets the defense value of this unit. </summary>
        public int Defense => _data.BaseDefense + (_data.GainDefense * (_level - 1));
        /// <summary> Gets the special attack value of this unit. </summary>
        public int SpecialAttack => _data.BaseSpecialAttack + (_data.GainSpecialAttack * (_level - 1));
        /// <summary> Gets the special defense value of this unit. </summary>
        public int SpecialDefense => _data.BaseSpecialDefense + (_data.GainSpecialDefense * (_level - 1));
        /// <summary> Gets the speed value of this unit. </summary>
        public int Speed => _data.BaseSpeed + (_data.GainSpeed * (_level - 1));
        /// <summary> Gets the movement points of this unit. </summary>
        public int MovementPoints => _data.MovementRange;
        /// <summary> Gets the jump height of this unit. </summary>
        public int JumpHeight => _data.JumpHeight;
        /// <summary> Gets the maximum fall height of this unit. </summary>
        public int FallHeight => _data.FallHeight;
        /// <summary>  Gets the list of skills for this unit. </summary>
        public List<SkillData> Skills => _data.Skills;

        /// <summary> Gets the movement patterns for each skill. </summary>
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

            foreach (var skill in Skills)
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
            => (Skills != null && index >= 0 && index < Skills.Count) ? Skills[index] : null;

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