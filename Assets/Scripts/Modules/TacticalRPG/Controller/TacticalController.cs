using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// Core tactical battle controller.
/// Handles unit selection, grid generation, turn flow, and input routing.
/// </summary>
public class TacticalController : Singleton<TacticalController>,
    IMoveHandler, ISubmitHandler, ICancelHandler
{
    private enum Team { Player, Enemy }

    [Header("Grid Settings")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TileData defaultTileData;
    [SerializeField] private GameObject cursor;
    [SerializeField] private Tilemap[] tilemaps;

    [Header("Units & UI")]
    [SerializeField] private List<Unit> allUnits = new();

    private Tile[,] grid;
    private Pathfinding pathfinding;
    private TacticalStateMachine stateMachine;

    private int width;
    private int height;
    private Team currentTeam = Team.Player;

    private readonly List<Unit> alliedUnits = new();
    private readonly List<Unit> enemyUnits = new();

    // ────────────────────────────────────────────────────────────────
    #region Events

    /// <summary>Fired whenever a unit becomes the current selection.</summary>
    public event System.Action<Unit> OnUnitSelectedEvent;

    /// <summary>Fired when a unit completes its action (move/attack/etc.).</summary>
    // public event System.Action<Unit> OnUnitActionFinishedEvent;

    /// <summary>Fired when the turn passes to a new team.</summary>
    public event System.Action<int> OnTurnChangedEvent;

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Properties

    public Unit SelectedUnit { get; private set; }
    public Tile[,] Grid => grid;
    public int Width => width;
    public int Height => height;
    public GameObject Cursor => cursor;
    public List<Unit> AllUnits => allUnits;
    public List<Unit> AlliedUnits => alliedUnits;
    public List<Unit> EnemyUnits => enemyUnits;
    public Pathfinding Pathfinding => pathfinding;

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();
        pathfinding = GetComponent<Pathfinding>();
        GenerateGrid();

        if (grid == null || grid.Length == 0)
            Debug.LogError("Grid initialization failed in TacticalController.");

        stateMachine = new TacticalStateMachine(this);
    }

    private void OnEnable()
    {
        Tile.OnTileClicked += (tile) => OnTileClicked(tile);
        Tile.OnTileHovered += (tile) => OnTileHovered(tile);
        Tile.OnTileHoverExited += () => OnTileHoverExited();
    }

    private void OnDisable()
    {
        Tile.OnTileClicked -= (tile) => OnTileClicked(tile);
        Tile.OnTileHovered -= (tile) => OnTileHovered(tile);
        Tile.OnTileHoverExited -= () => OnTileHoverExited();
    }

    private void Start()
    {
        InitializeUnits();
        SelectedUnit = null;
        currentTeam = Team.Player;
    }

    private void Update() => stateMachine?.Update();
    private void FixedUpdate() => stateMachine?.PhysicsUpdate();

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (var unit in allUnits)
        {
            unit.OnMovementComplete -= HandleUnitMovementComplete;
            // unit.OnActionComplete -= HandleUnitActionComplete;
        }
    }

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Input Handlers

    public void OnMove(AxisEventData eventData)
    {
        Vector2 move = eventData.moveVector;

        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
            stateMachine.CurrentState.HorizontalKey(move.x > 0 ? 1 : -1);
        else if (Mathf.Abs(move.y) > Mathf.Abs(move.x))
            stateMachine.CurrentState.VerticalKey(move.y > 0 ? 1 : -1);
    }

    public void OnSubmit(BaseEventData eventData) => stateMachine.CurrentState.ConfirmKey();

    public void OnCancel(BaseEventData eventData) => stateMachine.CurrentState.CancelKey();

    public void HandleMenuButtonClick(int buttonIndex) => stateMachine.CurrentState.OnClickButton(buttonIndex);

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Unit Management

    private void InitializeUnits()
    {
        alliedUnits.Clear();
        enemyUnits.Clear();

        foreach (var unit in allUnits)
        {
            unit.Initialize();

            // Subscribe to unit lifecycle events
            unit.OnMovementComplete += HandleUnitMovementComplete;
            // unit.OnActionComplete += HandleUnitActionComplete;

            switch (unit.Type)
            {
                case Unit.UnitType.Player:
                    alliedUnits.Add(unit);
                    break;
                case Unit.UnitType.Enemy:
                    enemyUnits.Add(unit);
                    break;
            }
        }

        foreach (var unit in allUnits)
            unit.SetAvailablePaths(pathfinding.GetAllPathsFrom(unit.GridPosition, unit));
    }

    public void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
        OnUnitSelectedEvent?.Invoke(unit);
    }

    /// <summary>
    /// Called when a unit finishes its movement phase.
    /// </summary>
    private void HandleUnitMovementComplete(Unit unit)
    {
        unit.MovementDone = true;

        // Movement is complete — trigger the next logical phase
        if (unit.Type == Unit.UnitType.Player)
            stateMachine.EnterState(stateMachine.MainMenuState);
        else
            EndTurn();
    }

    /// <summary>
    /// Called when a unit completes an action (attack, skill, etc.).
    /// </summary>
    private void HandleUnitActionComplete(Unit unit)
    {
        unit.ActionDone = true;

        // End the unit’s turn automatically if both actions are complete
        if (unit.MovementDone && unit.ActionDone)
            EndTurn();
    }

    public void MoveUnitPath(Unit unit, PathResult path)
    {
        if (!path.IsValid) return;

        stateMachine.EnterState(stateMachine.ActingUnitState);
        unit.FollowPath(path);
    }

    private void ResetUnits(IEnumerable<Unit> units)
    {
        foreach (var unit in units)
        {
            unit.EndTurn = false;
            unit.MovementDone = false;
            unit.ActionDone = false;
        }
    }

    public void EndTurn()
    {
        SelectedUnit.EndTurn = true;
        SelectedUnit = null;

        foreach (var unit in allUnits)
            unit.SetAvailablePaths(pathfinding.GetAllPathsFrom(unit.GridPosition, unit));

        switch (currentTeam)
        {
            case Team.Player:
                if (alliedUnits.Exists(u => !u.EndTurn))
                {
                    stateMachine.EnterState(stateMachine.UnitChoiceState);
                    return;
                }

                ResetUnits(enemyUnits);
                currentTeam = Team.Enemy;
                OnTurnChangedEvent?.Invoke((int)Team.Enemy);
                stateMachine.EnterState(stateMachine.EnemyTurnState);
                break;

            case Team.Enemy:
                if (enemyUnits.Exists(u => !u.EndTurn))
                {
                    stateMachine.EnterState(stateMachine.EnemyTurnState);
                    return;
                }

                ResetUnits(alliedUnits);
                currentTeam = Team.Player;
                OnTurnChangedEvent?.Invoke((int)Team.Player);
                stateMachine.EnterState(stateMachine.UnitChoiceState);
                break;
        }
    }

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Grid and Tile Management

    private void OnTileClicked(Tile tile) => stateMachine.CurrentState.OnTileClicked(tile);

    private void OnTileHovered(Tile tile) => stateMachine.CurrentState.OnTileHovered(tile);

    private void OnTileHoverExited() => stateMachine.CurrentState.OnTileHoverExited();

    public Tile GetTileAt(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= width || position.y >= height)
            return null;

        if (grid == null || grid.Length == 0)
        {
            Debug.LogError("Grid not initialized.");
            return null;
        }

        return grid[position.x, position.y];
    }

    public Tile GetTileAt(int x, int y) => GetTileAt(new Vector2Int(x, y));

    private void GenerateGrid()
    {
        if (tilemaps == null || tilemaps.Length == 0)
        {
            Debug.LogError("No tilemaps assigned in TacticalController.");
            return;
        }

        Tilemap baseMap = tilemaps[0];
        BoundsInt bounds = baseMap.cellBounds;

        width = bounds.size.x;
        height = bounds.size.y;
        grid = new Tile[width, height];

        int tileCount = 0;

        foreach (Tilemap tilemap in tilemaps)
        {
            int sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder;

            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tileBase = tilemap.GetTile(pos);
                if (tileBase == null) continue;

                int x = pos.x - tilemap.cellBounds.xMin;
                int y = pos.y - tilemap.cellBounds.yMin;
                int z = pos.z;

                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }

                Vector3 worldPos = tilemap.CellToWorld(pos) + new Vector3(0, 0.25f, 0);
                GameObject tileObj = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                tileObj.name = $"Tile_{x}_{y}_H{z}";

                Tile tile = tileObj.GetComponent<Tile>();
                tile.Initialize(defaultTileData, new Vector2Int(x, y), z, sortingOrder);
                grid[x, y] = tile;
                tileCount++;
            }
        }
    }

    public void ResetAllTiles()
    {
        foreach (var tile in grid)
        {
            if (tile != null)
                tile.ResetIllumination();
        }
    }

    #endregion
}