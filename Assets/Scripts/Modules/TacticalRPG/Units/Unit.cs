using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Represents a unit in the tactical grid, capable of moving across tiles and using skills.
/// </summary>
public class Unit : MonoBehaviour
{
    public enum UnitType
    {
        Player,
        Enemy,
        Neutral
    }

    [Header("Unit Settings")]
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private int movementPoints;
    [SerializeField] private int jumpHeight = 1;
    [SerializeField] private int maxFallHeight = 10;
    [SerializeField] private UnitType unitType = UnitType.Player;
    [SerializeField] private List<SkillData> skills = new();

    private Tile currentTile = null;
    private Tile previousTile = null;
    private SpriteRenderer spriteRenderer;
    private PathResult pathToFollow;
    public bool endTurn = false;
    private bool actionDone = false;
    private bool movementDone = false;
    private List<List<Vector2Int>> movementPatterns = new();

    /// <summary>
    /// List of available movement paths for this unit.
    /// </summary>
    public List<PathResult> AvailablePaths { get; private set; } = new();

    /// <summary>
    /// The tile the unit is currently occupying.
    /// </summary>
    public Tile CurrentTile => currentTile;

    /// <summary>
    /// The current grid position of the unit.
    /// </summary>
    public Vector2Int GridPosition => currentTile != null ? currentTile.GridPosition : startPosition;

    /// <summary>
    /// The tile the unit previously occupied.
    /// </summary>
    public Tile PreviousTile => previousTile;

    /// <summary>
    /// Remaining movement points.
    /// </summary>
    public int MovementPoints => movementPoints;

    /// <summary>
    /// Maximum height the unit can jump.
    /// </summary>
    public int JumpHeight => jumpHeight;

    /// <summary>
    /// Maximum height the unit can fall without penalty.
    /// </summary>
    public int MaxFallHeight => maxFallHeight;

    /// <summary>
    /// The type of the unit (Player, Enemy, Neutral).
    /// </summary>
    public UnitType Type => unitType;

    /// <summary>
    /// List of skills the unit possesses.
    /// </summary>
    public List<SkillData> Skills => skills;

    /// <summary>
    /// Precomputed movement patterns based on skills.
    /// </summary>
    public List<List<Vector2Int>> MovementPatterns => movementPatterns;

    /// <summary>
    /// Indicates whether the unit has ended its turn.
    /// </summary>
    public bool EndTurn
    {
        get => endTurn;
        set => endTurn = value;
    }

    /// <summary>
    /// Indicates whether the unit has completed its movement for the turn.
    /// </summary>
    public bool MovementDone
    {
        get => movementDone;
        set => movementDone = value;
    }

    /// <summary>
    /// Indicates whether the unit has completed its action for the turn.
    /// </summary>
    public bool ActionDone
    {
        get => actionDone;
        set => actionDone = value;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        
    }

    public void Initialize()
    {
        currentTile = TacticalController.Instance.GetTileAt(startPosition);
        currentTile.OccupyingUnit = this;

        previousTile = null;

        //GridPosition = currentTile.GridPosition;

        Debug.Log($"Unit {name} starting at position {GridPosition}");

        Vector3 tilePosition = currentTile.transform.position;
        transform.position = new Vector3(tilePosition.x, tilePosition.y + 0.3f, 0);
        spriteRenderer.sortingOrder = currentTile.Order + 2;

        foreach (SkillData skill in skills)
        {
            if (skill != null)
                movementPatterns.Add(skill.AreaOfEffect.GetAllRangedPositions());
        }
    }

    /// <summary>
    /// Starts moving the unit along a given path.
    /// </summary>
    /// <param name="pathResult">The path the unit should follow.</param>
    public void GetPath(PathResult pathResult)
    {
        if (!pathResult.IsValid)
        {
            Debug.LogError($"Path is empty or null for unit {name}.");
            return;
        }

        pathToFollow = pathResult;
        StartCoroutine(MoveAlongPath());
    }

    /// <summary>
    /// Moves the unit step by step along the assigned path.
    /// </summary>
    private IEnumerator MoveAlongPath()
    {
        //Tile t;

        foreach (Tile tile in pathToFollow.Path)
        {
            Vector3 targetPosition = new(tile.transform.position.x, tile.transform.position.y + 0.3f, 0);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 3f);
                yield return null;
            }

            transform.position = targetPosition;
            spriteRenderer.sortingOrder = tile.Order + 2;

            //t = tile;
        }

        //currentTile = TacticalController.Instance.GetTileAt(gridPosition);
        //currentTile.OccupyingUnit = this;

        SetPosition(pathToFollow.Path[^1]);

        movementDone = true;

        TacticalController.Instance.OnUnitFinishedAction(this);
    }

    /// <summary>
    /// Assigns available movement paths to this unit.
    /// </summary>
    /// <param name="paths">List of possible paths.</param>
    public void SetAvailablePaths(List<PathResult> paths)
    {
        if (paths != null && paths.Count > 0)
            AvailablePaths = paths;
        else
            AvailablePaths = new List<PathResult>();
    }

    /// <summary>
    /// Gets a skill by its index.
    /// </summary>
    /// <param name="index">The index of the skill.</param>
    /// <returns>The skill at the given index, or null if invalid.</returns>
    public SkillData GetSkillByIndex(int index)
    {
        if (skills != null && index >= 0 && index < skills.Count)
            return skills[index];

        return null;
    }

    public void SetPosition(Tile newTile)
    {
        previousTile = currentTile;
        currentTile.OccupyingUnit = null;

        currentTile = newTile;
        currentTile.OccupyingUnit = this;

        //GridPosition = currentTile.GridPosition;

        Vector3 tilePosition = currentTile.transform.position;
        transform.position = new Vector3(tilePosition.x, tilePosition.y + 0.3f, 0);
        spriteRenderer.sortingOrder = currentTile.Order + 2;
    }

    public void SetPosition(Vector2Int newPosition)
    {
        Tile newTile = TacticalController.Instance.GetTileAt(newPosition);

        SetPosition(newTile);
    }
}