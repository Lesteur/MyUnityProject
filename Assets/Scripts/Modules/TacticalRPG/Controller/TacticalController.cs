using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the tactical grid, units, and state machine. Handles input and unit interactions.
/// </summary>
public class TacticalController : Singleton<TacticalController>, IMoveHandler, ISubmitHandler, ICancelHandler, IPointerClickHandler
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TileData defaultTileData;
    [SerializeField] private GameObject cursor;
    [SerializeField] private List<Tilemap> tilemaps = new();

    [Header("Units & UI")]
    [SerializeField] private List<Unit> units = new();

    private Tile[,] grid;
    private Pathfinding pathfinding;
    private TacticalStateMachine stateMachine;
    private int width;
    private int height;
    private int turnTeam = 0; // 0 = Player, 1 = Enemy
    private List<Unit> alliedUnits = new();
    private List<Unit> enemyUnits = new();

    /// <summary>
    /// Currently selected unit on the grid.
    /// </summary>
    public Unit SelectedUnit { get; private set; }

    /// <summary>
    /// Provides access to the tactical grid.
    /// </summary>
    public Tile[,] Grid => grid;

    /// <summary>
    /// Width of the grid in tiles.
    /// </summary>
    public int Width => width;

    /// <summary>
    /// Height of the grid in tiles.
    /// </summary>
    public int Height => height;

    /// <summary>
    /// Cursor GameObject for indicating selection.
    /// </summary>
    public GameObject Cursor => cursor;

    /// <summary>
    /// List of all units on the battlefield.
    /// </summary>
    public List<Unit> Units => units;

    /// <summary>
    /// List of allied (player) units.
    /// </summary>
    public List<Unit> AlliedUnits => alliedUnits;

    /// <summary>
    /// List of enemy units.
    /// </summary>
    public List<Unit> EnemyUnits => enemyUnits;

    /// <summary>
    /// Reference to the pathfinding system.
    /// </summary>
    public Pathfinding Pathfinding => pathfinding;

    protected override void Awake()
    {
        base.Awake();

        pathfinding = GetComponent<Pathfinding>();

        GenerateGrid();

        if (grid == null || grid.Length == 0)
            Debug.LogError("Grid has not been initialized properly in TacticalController.");

        stateMachine = new TacticalStateMachine(this);
    }

    private void Start()
    {
        foreach (Unit unit in units)
            unit.Initialize();

        foreach (Unit unit in units)
        {
            List<PathResult> unitPaths = pathfinding.GetAllPathsFrom(unit.GridPosition, unit);
            unit.SetAvailablePaths(unitPaths);

            if (unit != null)
            {
                if (unit.Type == Unit.UnitType.Player)
                    alliedUnits.Add(unit);
                else if (unit.Type == Unit.UnitType.Enemy)
                    enemyUnits.Add(unit);
            }
        }

        turnTeam = 0;
        SelectedUnit = null;
    }

    private void Update()
    {
        if (stateMachine == null) return;

        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if (stateMachine == null) return;

        stateMachine.PhysicsUpdate();
    }

    public void OnMove(AxisEventData eventData)
    {
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
        stateMachine.CurrentState.ConfirmKey();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Pointer Clicked on TacticalController");

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        stateMachine.CurrentState.ConfirmKey();
    }

    public void OnCancel(BaseEventData eventData)
    {
        stateMachine.CurrentState.CancelKey();
    }

    /// <summary>
    /// Handles button clicks from the tactical menu.
    /// </summary>
    /// <param name="buttonIndex">The index of the clicked button.</param>
    public void OnClickButton(int buttonIndex)
    {
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
        /*
        foreach (Unit unit in units)
        {
            List<PathResult> unitPaths = pathfinding.GetAllPathsFrom(unit.GridPosition, unit);
            unit.SetAvailablePaths(unitPaths);
        }
        */

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
        }
    }

    /// <summary>
    /// Selects a unit on the battlefield.
    /// </summary>
    /// <param name="unit">The unit to select.</param>
    public void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
    }

    /// <summary>
    /// Generates the isometric grid and initializes tiles using the tilemap.
    /// </summary>
    private void GenerateGrid()
    {
        int index = 0;
        var tm = tilemaps[0];

        BoundsInt tilemapBounds = tm.cellBounds;
        width = tilemapBounds.size.x;
        height = tilemapBounds.size.y;

        grid = new Tile[width, height];

        foreach (Tilemap tilemap in tilemaps)
        {
            tilemapBounds = tilemap.cellBounds;

            int sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder;

            foreach (Vector3Int pos in tilemapBounds.allPositionsWithin)
            {
                TileBase tileBase = tilemap.GetTile(pos);

                if (tileBase == null)
                    continue;

                int x = pos.x - tilemapBounds.xMin;
                int y = pos.y - tilemapBounds.yMin;
                int z = pos.z; // Use the z-coordinate as height

                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }

                Vector3 position = tilemap.CellToWorld(pos) + new Vector3(0, 0.25f, 0);

                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileObject.name = $"Tile_{x}_{y}_H{z}";

                Tile tile = tileObject.GetComponent<Tile>();
                tile.Initialize(defaultTileData, new Vector2Int(x, y), z, sortingOrder);
                grid[x, y] = tile;

                index ++;
            }
        }
        
        Debug.Log($"Generated grid with dimensions: {width}x{height} and total tiles: {index}");
    }
}