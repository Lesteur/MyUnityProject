using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using Game.Input;

public class TacticalController : MonoBehaviour
{
    public int width;
    public int height;
    public int tileSize = 1; // Size of each tile in world units
    public GameObject tilePrefab;
    public TileData defaultTileData;
    public List<Unit> units;
    public Unit selectedUnit;

    public Tile[,] grid { get; private set; }
    public Pathfinding pathfinding { get; private set; }

    public BaseMenu actionMenu;
    public BaseMenu skillMenu;

    private bool isActive = true;

    private TacticalStateMachine stateMachine;

    private InputAction horizontalAction    => InputReader.Instance.inputActions.Global.Horizontal;
    private InputAction verticalAction      => InputReader.Instance.inputActions.Global.Vertical;
    private InputAction confirmAction       => InputReader.Instance.inputActions.Global.Confirm;
    private InputAction backAction          => InputReader.Instance.inputActions.Global.Back;

    private void Awake()
    {
        Debug.Log("TacticalController Start called.");

        pathfinding = GetComponent<Pathfinding>();

        GenerateGrid();

        if (grid == null || grid.Length == 0)
        {
            Debug.LogError("Grid has not been initialized properly.");
        }

        stateMachine = new TacticalStateMachine(this);
    }

    private void Start()
    {
        // Get all available paths for each unit
        foreach (Unit unit in units)
        {
            List<PathResult> unitPaths = pathfinding.GetAllPathsFrom(unit.gridPosition, unit);
            unit.SetAvailablePaths(unitPaths);
        }

        selectedUnit = null;

        actionMenu.Hide();
    }

    private void OnEnable()
    {
        if (InputReader.Instance == null)
        {
            Debug.LogError("InputReader instance is null. Ensure InputReader is initialized before TacticalController.");
            return;
        }

        InputReader.Instance.horizontalEvent    += horizontalEvent;
        InputReader.Instance.verticalEvent      += verticalEvent;
        InputReader.Instance.confirmEvent       += confirmEvent;
        InputReader.Instance.backEvent          += cancelEvent;
    }

    private void OnDisable()
    {
        InputReader.Instance.horizontalEvent    -= horizontalEvent;
        InputReader.Instance.verticalEvent      -= verticalEvent;
        InputReader.Instance.confirmEvent       -= confirmEvent;
        InputReader.Instance.backEvent          -= cancelEvent;
    }

    private void Update()
    {
        if (InputReader.Instance == null || stateMachine == null)
            return;

        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if (InputReader.Instance == null || stateMachine == null)
            return;

        stateMachine.PhysicsUpdate();
    }

    private void horizontalEvent(int direction)
    {
        stateMachine.currentState.HorizontalKey(direction);
    }

    private void verticalEvent(int direction)
    {
        stateMachine.currentState.VerticalKey(direction);
    }

    private void confirmEvent()
    {
        stateMachine.currentState.ConfirmKey();
    }

    private void cancelEvent()
    {
        stateMachine.currentState.CancelKey();
    }

    public void OnClickButton(int buttonIndex)
    {
        Debug.Log($"Button {buttonIndex} clicked.");
        stateMachine.currentState.OnClickButton(buttonIndex);
    }

    public Tile GetTileAt(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= width || position.y >= height)
            return null;

        if (grid == null || grid.Length == 0)
        {
            Debug.LogError("Grid has not been initialized.");
        }

        return grid[position.x, position.y];
    }

    public Tile GetTileAt(int x, int y)
    {
        return GetTileAt(new Vector2Int(x, y));
    }

    private void GenerateGrid()
    {
        int index = 0;
        int orderOffset = 150; // Offset to ensure tiles are rendered in the correct order

        grid = new Tile[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int trueHeight = 0; //Random.Range(0, 2); // Random height for demonstration, can be replaced with actual logic

                if (y == 0)
                    trueHeight = x; // Set height based on x for the first row
                else
                {
                    if (x % 2 == 0 && y % 4 == 0)
                        trueHeight = 7;
                }

                GameObject tileObject = null;

                for (int z = 0; z < trueHeight + 1; z++)
                {
                    Vector3 position = new Vector3(transform.position.x + 0.5f * (x - y), transform.position.y - 0.25f * (x + y) + z * 0.25f, 0);
                    tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);

                    tileObject.name = $"Tile_{x}_{y}_H{z}";
                    tileObject.GetComponent<SpriteRenderer>().sortingOrder = (x + y) * orderOffset + z * 15;
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

    public void OnUnitFinishedAction(Unit finishedUnit)
    {
        // Get all available paths for each unit
        foreach (Unit unit in units)
        {
            List<PathResult> unitPaths = pathfinding.GetAllPathsFrom(unit.gridPosition, unit);
            unit.SetAvailablePaths(unitPaths);
        }

        stateMachine.EnterState(stateMachine.unitActionState);

        isActive = true; // Reactivate the grid manager after the unit finishes moving
    }

    public void MoveUnitPath(Unit unit, PathResult path)
    {
        if (!isActive)
            return;

        if (path != null)
        {
            isActive = false; // Deactivate the grid manager after confirming the path

            unit.GetPath(path);
            Debug.Log($"Unit {unit.name} is moving to {path.destination.gridPosition} along the path.");
        }
        else
        {
            Debug.LogWarning("No valid path selected for confirmation.");
        }
    }

    public void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        Debug.Log($"Unit {unit.name} selected at position {unit.gridPosition}.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Draw grid lines for visualization
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Draw isometric grid lines
                Vector3 start = new Vector3(transform.position.x + 0.5f * (x - y), transform.position.y - 0.25f * (x + y) + 0.5f, 0);
                Vector3 endX = new Vector3(transform.position.x + 0.5f * (x + 1 - y), transform.position.y - 0.25f * (x + 1 + y) + 0.5f, 0);
                Vector3 endY = new Vector3(transform.position.x + 0.5f * (x - (y + 1)), transform.position.y - 0.25f * (x + (y + 1)) + 0.5f, 0);
                Gizmos.DrawLine(start, endX);
                Gizmos.DrawLine(start, endY);
            }
        }
    }
}