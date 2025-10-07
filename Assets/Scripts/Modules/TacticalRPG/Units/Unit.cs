using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private readonly List<List<Vector2Int>> _movementPatterns = new();

    // ────────────────────────────────────────────────────────────────
    #region Events

    /// <summary>Fired when the unit finishes its movement animation and reaches destination.</summary>
    public event System.Action<Unit> OnMovementComplete;

    /// <summary>Fired when the unit finishes its action (attack, skill, etc.).</summary>
    // public event System.Action<Unit> OnActionComplete;

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Properties

    public bool EndTurn { get; set; }
    public bool MovementDone { get; set; }
    public bool ActionDone { get; set; }

    public List<PathResult> AvailablePaths { get; private set; } = new();
    public Tile CurrentTile         => _currentTile;
    public Tile PreviousTile        => _previousTile;
    public Vector2Int GridPosition  => _currentTile != null ? _currentTile.GridPosition : _startPosition;
    public int MovementPoints       => _movementPoints;
    public int JumpHeight           => _jumpHeight;
    public int MaxFallHeight        => _maxFallHeight;
    public UnitType Type            => _unitType;
    public List<SkillData> Skills   => _skills;
    public List<List<Vector2Int>> MovementPatterns => _movementPatterns;

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Initialization

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

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
                _movementPatterns.Add(skill.AreaOfEffect.GetAllRangedPositions());
        }
    }

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Movement

    /// <summary>
    /// Starts moving the unit along the given path using coroutine animation.
    /// </summary>
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

    private void MoveToTile(Tile tile)
    {
        if (tile == null) return;

        Vector3 pos = tile.transform.position;
        transform.position = new Vector3(pos.x, pos.y + 0.3f, 0);
        _spriteRenderer.sortingOrder = tile.Order + 2;
    }

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Position & Skills

    public void SetAvailablePaths(List<PathResult> paths)
        => AvailablePaths = (paths != null && paths.Count > 0) ? paths : new();

    public SkillData GetSkillByIndex(int index)
        => (_skills != null && index >= 0 && index < _skills.Count) ? _skills[index] : null;

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

    public void SetPosition(Vector2Int newPosition)
    {
        var tile = TacticalController.Instance.GetTileAt(newPosition);
        SetPosition(tile);
    }

    #endregion
}