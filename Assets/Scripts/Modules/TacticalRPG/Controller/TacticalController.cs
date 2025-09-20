using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Game.Input;

/// <summary>
/// Manages the tactical grid, units, and state machine. Handles input and unit interactions.
/// </summary>
public class TacticalController : MonoBehaviour, IMoveHandler, ISubmitHandler, ICancelHandler
{
    [Header("Grid Settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TileData defaultTileData;
    [SerializeField] private GameObject cursor;

    [Header("Units & UI")]
    [SerializeField] private List<Unit> units = new();
    [SerializeField] private TacticalMenu tacticalMenu;

    private Tile[,] grid;
    private Pathfinding pathfinding;
    private TacticalStateMachine stateMachine;

    /// <summary>
    /// Currently selected unit on the grid.
    /// </summary>
    public Unit SelectedUnit { get; private set; }

    /// <summary>
    /// Provides access to the tactical grid.
    /// </summary>
    public Tile[,] Grid => grid;

    /// <summary>
    /// Cursor GameObject for indicating selection.
    /// </summary>
    public GameObject Cursor => cursor;

    /// <summary>
    /// List of all units on the battlefield.
    /// </summary>
    public List<Unit> Units => units;

    /// <summary>
    /// Reference to the tactical menu UI.
    /// </summary>
    public TacticalMenu TacticalMenu => tacticalMenu;

    /// <summary>
    /// Reference to the pathfinding system.
    /// </summary>
    public Pathfinding Pathfinding => pathfinding;

    private void Awake()
    {
        Debug.Log("TacticalController Awake called.");

        pathfinding = GetComponent<Pathfinding>();
        GenerateGrid();

        if (grid == null || grid.Length == 0)
            Debug.LogError("Grid has not been initialized properly.");

        stateMachine = new TacticalStateMachine(this);
    }

    private void Start()
    {
        foreach (Unit unit in units)
        {
            List<PathResult> unitPaths = pathfinding.GetAllPathsFrom(unit.GridPosition, unit);
            unit.SetAvailablePaths(unitPaths);
        }

        SelectedUnit = null;
    }

    private void OnEnable()
    {
        if (InputReader.Instance == null)
        {
            Debug.LogError("InputReader instance is null. Ensure InputReader is initialized before TacticalController.");
            return;
        }
    }

    private void OnDisable()
    {
        if (InputReader.Instance == null) return;
    }

    private void Update()
    {
        if (stateMachine == null || InputReader.Instance == null) return;
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if (stateMachine == null || InputReader.Instance == null) return;
        stateMachine.PhysicsUpdate();
    }

    public void OnMove(AxisEventData eventData)
    {
        Debug.Log("OnMove event received.");

        Vector2 move = eventData.moveVector;

        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            int direction = (move.x > 0) ? 1 : -1;
            stateMachine.CurrentState.HorizontalKey(direction);
        }
        else if (Mathf.Abs(move.y) > Mathf.Abs(move.x))
        {
            int direction = (move.y > 0) ? 1 : -1;
            stateMachine.CurrentState.VerticalKey(direction);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Debug.Log("OnSubmit event received.");

        stateMachine.CurrentState.ConfirmKey();
    }

    public void OnCancel(BaseEventData eventData)
    {
        Debug.Log("OnCancel event received.");

        stateMachine.CurrentState.CancelKey();
    }

    /// <summary>
    /// Handles button clicks from the tactical menu.
    /// </summary>
    /// <param name="buttonIndex">The index of the clicked button.</param>
    public void OnClickButton(int buttonIndex)
    {
        Debug.Log($"Button {buttonIndex} clicked.");
        stateMachine.CurrentState.OnClickButton(buttonIndex);
    }

    /// <summary>
    /// Retrieves the tile at the given grid position.
    /// </summary>
    /// <param name="position">Grid coordinates.</param>
    /// <returns>The tile at the position, or null if invalid.</returns>
    public Tile GetTileAt(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= width || position.y >= height)
            return null;

        if (grid == null || grid.Length == 0)
            Debug.LogError("Grid has not been initialized.");

        return grid[position.x, position.y];
    }

    /// <summary>
    /// Retrieves the tile at the given coordinates.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    public Tile GetTileAt(int x, int y) => GetTileAt(new Vector2Int(x, y));

    /// <summary>
    /// Called when a unit finishes its movement or action.
    /// Updates available paths and state machine.
    /// </summary>
    /// <param name="finishedUnit">The unit that finished its action.</param>
    public void OnUnitFinishedAction(Unit finishedUnit)
    {
        foreach (Unit unit in units)
        {
            List<PathResult> unitPaths = pathfinding.GetAllPathsFrom(unit.GridPosition, unit);
            unit.SetAvailablePaths(unitPaths);
        }

        stateMachine.EnterState(stateMachine.MainMenuState);
    }

    /// <summary>
    /// Moves a unit along the given path, if valid.
    /// </summary>
    /// <param name="unit">The unit to move.</param>
    /// <param name="path">The path result.</param>
    public void MoveUnitPath(Unit unit, PathResult path)
    {
        if (path.IsValid)
        {
            stateMachine.EnterState(stateMachine.ActingUnitState);
            unit.GetPath(path);
            Debug.Log($"Unit {unit.name} is moving to {path.Destination.GridPosition}.");
        }
        else
        {
            Debug.LogWarning("No valid path selected for confirmation.");
        }
    }

    /// <summary>
    /// Selects a unit on the battlefield.
    /// </summary>
    /// <param name="unit">The unit to select.</param>
    public void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
        Debug.Log($"Unit {unit.name} selected at position {unit.GridPosition}.");
    }

    /// <summary>
    /// Generates the isometric grid and initializes tiles.
    /// </summary>
    private void GenerateGrid()
    {
        int index = 0;
        int orderOffset = 150;

        grid = new Tile[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int trueHeight = (y == 0) ? x : 0;

                if (x % 3 == 0 && y % 3 == 0)
                    trueHeight = 5;

                GameObject tileObject = null;

                for (int z = 0; z < trueHeight + 1; z++)
                {
                    Vector3 position = new(
                        transform.position.x + 0.5f * (x - y),
                        transform.position.y - 0.25f * (x + y) + z * 0.25f,
                        0);

                    tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    tileObject.name = $"Tile_{x}_{y}_H{z}";
                    tileObject.GetComponent<SpriteRenderer>().sortingOrder = (x + y) * orderOffset + z * 15;

                    if (z < trueHeight)
                    {
                        var polygonCollider = tileObject.GetComponent<PolygonCollider2D>();

                        if (polygonCollider != null)
                            Destroy(polygonCollider);
                    }
                }

                Tile tile = tileObject.GetComponent<Tile>();
                if (tile == null)
                {
                    Debug.LogError("Tile prefab must have a Tile component attached.");
                    continue;
                }

                tile.Initialize(defaultTileData, new Vector2Int(x, y), trueHeight, index);
                grid[x, y] = tile;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 start = new(transform.position.x + 0.5f * (x - y), transform.position.y - 0.25f * (x + y) + 0.5f, 0);
                Vector3 endX = new(transform.position.x + 0.5f * (x + 1 - y), transform.position.y - 0.25f * (x + 1 + y) + 0.5f, 0);
                Vector3 endY = new(transform.position.x + 0.5f * (x - (y + 1)), transform.position.y - 0.25f * (x + (y + 1)) + 0.5f, 0);

                Gizmos.DrawLine(start, endX);
                Gizmos.DrawLine(start, endY);
            }
        }
    }
}