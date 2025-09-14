using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Represents a unit in the tactical grid, capable of moving across tiles and using skills.
/// </summary>
public class Unit : MonoBehaviour
{
    [Header("Unit Settings")]
    [SerializeField] private Vector2Int gridPosition;
    [SerializeField] private int movementPoints;
    [SerializeField] private int jumpHeight = 1;
    [SerializeField] private int maxFallHeight = 10;
    [SerializeField] private List<SkillData> skills = new();

    [Header("Runtime References")]
    [SerializeField] private Tile currentTile;

    private SpriteRenderer spriteRenderer;
    private TacticalController tacticalController;
    private PathResult pathToFollow;

    /// <summary>
    /// List of available movement paths for this unit.
    /// </summary>
    public List<PathResult> AvailablePaths { get; private set; } = new();

    /// <summary>
    /// The current grid position of the unit.
    /// </summary>
    public Vector2Int GridPosition => gridPosition;

    /// <summary>
    /// The tile the unit is currently occupying.
    /// </summary>
    public Tile CurrentTile => currentTile;

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
    /// List of skills the unit possesses.
    /// </summary>
    public List<SkillData> Skills => skills;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tacticalController = FindFirstObjectByType<TacticalController>();

        currentTile = tacticalController.GetTileAt(gridPosition);
        currentTile.OccupyingUnit = this;
    }

    private void Start()
    {
        Vector3 tilePosition = currentTile.transform.position;
        transform.position = new Vector3(tilePosition.x, tilePosition.y + 0.4f, 0);

        spriteRenderer.sortingOrder = currentTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    /// <summary>
    /// Starts moving the unit along a given path.
    /// </summary>
    /// <param name="pathResult">The path the unit should follow.</param>
    public void GetPath(PathResult pathResult)
    {
        if (pathResult == null || pathResult.Path == null || pathResult.Path.Count == 0)
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
        currentTile.OccupyingUnit = null;

        foreach (Tile tile in pathToFollow.Path)
        {
            Vector3 targetPosition = new(tile.transform.position.x, tile.transform.position.y + 0.4f, 0);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 3f);
                yield return null;
            }

            transform.position = targetPosition;
            spriteRenderer.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;

            gridPosition = tile.GridPosition;
        }

        currentTile = tacticalController.GetTileAt(gridPosition);
        currentTile.OccupyingUnit = this;

        tacticalController.OnUnitFinishedAction(this);
    }

    /// <summary>
    /// Assigns available movement paths to this unit.
    /// </summary>
    /// <param name="paths">List of possible paths.</param>
    public void SetAvailablePaths(List<PathResult> paths)
    {
        if (paths != null && paths.Count > 0)
        {
            AvailablePaths = paths;
            Debug.Log($"Unit {name} has {AvailablePaths.Count} available paths.");
        }
        else
        {
            Debug.LogWarning($"No available paths provided for unit {name}.");
            AvailablePaths = new List<PathResult>();
        }
    }

    /// <summary>
    /// Gets a skill by its index.
    /// </summary>
    /// <param name="index">The index of the skill.</param>
    /// <returns>The skill at the given index, or null if invalid.</returns>
    public SkillData GetSkillByIndex(int index)
    {
        if (skills != null && index >= 0 && index < skills.Count)
        {
            return skills[index];
        }

        Debug.LogWarning($"Skill index {index} is out of range for unit {name}.");
        return null;
    }
}