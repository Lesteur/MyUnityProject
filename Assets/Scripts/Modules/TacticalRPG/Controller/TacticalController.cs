using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the tactical grid, units, and game state transitions.
/// Handles input, turn flow, and unit interactions.
/// </summary>
public class TacticalController : Singleton<TacticalController>, 
    IMoveHandler, ISubmitHandler, ICancelHandler, IPointerClickHandler
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

    public event System.Action<Unit> OnUnitSelectedEvent;
    public event System.Action<Unit> OnUnitActionFinishedEvent;
    public event System.Action<int> OnTurnChangedEvent;


    public Unit SelectedUnit { get; private set; }
    public Tile[,] Grid => grid;
    public int Width => width;
    public int Height => height;
    public GameObject Cursor => cursor;
    public List<Unit> AllUnits => allUnits;
    public List<Unit> AlliedUnits => alliedUnits;
    public List<Unit> EnemyUnits => enemyUnits;
    public Pathfinding Pathfinding => pathfinding;

    // ────────────────────────────────────────────────────────────────
    protected override void Awake()
    {
        base.Awake();

        pathfinding = GetComponent<Pathfinding>();
        GenerateGrid();

        if (grid == null || grid.Length == 0)
            Debug.LogError("Grid initialization failed in TacticalController.");

        stateMachine = new TacticalStateMachine(this);
    }

    private void Start()
    {
        InitializeUnits();
        SelectedUnit = null;
        currentTeam = Team.Player;
    }

    private void Update()
    {
        stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        stateMachine?.PhysicsUpdate();
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            stateMachine.CurrentState.ConfirmKey();
    }

    public void OnCancel(BaseEventData eventData) => stateMachine.CurrentState.CancelKey();

    public void HandleMenuButtonClick(int buttonIndex)
        => stateMachine.CurrentState.OnClickButton(buttonIndex);

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Unit Management

    private void InitializeUnits()
    {
        foreach (var unit in allUnits)
        {
            unit.Initialize();

            if (unit.Type == Unit.UnitType.Player)
                alliedUnits.Add(unit);
            else if (unit.Type == Unit.UnitType.Enemy)
                enemyUnits.Add(unit);
        }

        foreach (var unit in allUnits)
            unit.SetAvailablePaths(pathfinding.GetAllPathsFrom(unit.GridPosition, unit));
    }

    public void SelectUnit(Unit unit) => SelectedUnit = unit;

    public void HandleUnitActionEnd()
    {
        if (currentTeam == Team.Player)
        {
            stateMachine.EnterState(stateMachine.MainMenuState);
        }
        else
        {
            SelectedUnit.EndTurn = true;
            SelectedUnit.MovementDone = true;
            SelectedUnit.ActionDone = true;
            EndTurn();
        }
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
        if (SelectedUnit != null)
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
                stateMachine.EnterState(stateMachine.UnitChoiceState);
                break;
        }
    }

    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Grid and Tile Management

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

        Debug.Log($"Generated grid: {width}x{height}, total tiles: {tileCount}");
    }

    #endregion
}