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
    public Unit currentUnit;

    public Tile[,] grid { get; private set; }
    public Pathfinding pathfinding { get; private set; }
    public List<PathResult> paths { get; private set; }
    public PathResult currentPath { get; private set; }

    public Vector2Int currentPosition = Vector2Int.zero;
    public Vector2Int newPosition = Vector2Int.zero;

    public BaseMenu actionMenu;

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
        currentPosition = currentUnit != null ? currentUnit.gridPosition : Vector2Int.zero;
        newPosition = currentPosition; // Initialize new position to current position

        // Get all paths from the current position
        paths = pathfinding.GetAllPathsFrom(currentPosition, currentUnit);

        // Initialize the current path if available
        if (paths.Count > 0)
        {
            currentPath = null;
        }
        else
        {
            Debug.LogWarning("No paths found from the current position.");
        }

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

    public void OnUnitFinishedAction(Unit currentUnit)
    {
        // Reset the new position to the current position after the unit finishes moving
        newPosition = currentUnit.gridPosition;
        Debug.Log($"Unit {currentUnit.name} finished moving. Resetting new position to {newPosition}.");

        paths = pathfinding.GetAllPathsFrom(newPosition, currentUnit);
        currentPath = null; // Reset current path
        currentPosition = newPosition; // Update current position to the new position

        stateMachine.EnterState(stateMachine.unitsState);

        isActive = true; // Reactivate the grid manager after the unit finishes moving
    }

    public void MoveCursorTile(int x, int y)
    {
        if (!isActive)
            return;

        newPosition.x = Mathf.Clamp(newPosition.x + x, 0, width - 1);
        newPosition.y = Mathf.Clamp(newPosition.y + y, 0, height - 1);

        bool pathFound = false;

        foreach (var path in paths)
        {
            if (path.destination.gridPosition == newPosition)
            {
                pathFound = true;
                currentPath = path;
                break;
            }
        }

        if (!pathFound)
            currentPath = null; // Reset current path if no valid path found
    }

    public void MoveUnitPath()
    {
        if (!isActive)
            return;

        if (currentPath != null)
        {
            isActive = false; // Deactivate the grid manager after confirming the path

            currentUnit.GetPath(currentPath);
            Debug.Log($"Unit {currentUnit.name} is moving to {currentPath.destination.gridPosition} along the path.");
        }
        else
        {
            Debug.LogWarning("No valid path selected for confirmation.");
        }
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