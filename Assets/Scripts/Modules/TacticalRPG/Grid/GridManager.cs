using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

using Game.Input;

public class GridManager : MonoBehaviour
{
    public int width;
    public int height;
    public int tileSize = 1; // Size of each tile in world units
    public GameObject tilePrefab;
    public TileData defaultTileData;
    public Unit currentUnit;

    private Tile[,] grid;
    private Pathfinding pathfinding;
    private List<PathResult> paths;
    private PathResult currentPath;
    private Vector2Int currentPosition = Vector2Int.zero;
    private Vector2Int newPosition = Vector2Int.zero;
    private bool isActive = true;

    private void Awake()
    {
        pathfinding = GetComponent<Pathfinding>();

        Debug.Log("GridManager Start called.");
        GenerateGrid();

        if (grid == null || grid.Length == 0)
        {
            Debug.LogError("Grid has not been initialized properly.");
        }
    }

    private void Start()
    {
        currentPosition = currentUnit != null ? currentUnit.gridPosition : Vector2Int.zero;
        newPosition = currentPosition; // Initialize new position to current position

        int moveRange = currentUnit != null ? currentUnit.movementPoints : 5; // Default move range if no unit is set
        int heightJump = currentUnit != null ? currentUnit.jumpHeight : 1; //
        int maxFallHeight = currentUnit != null ? currentUnit.maxFallHeight : 10; // Default max fall height if no unit is set

        // Get all paths from the current position
        paths = pathfinding.GetAllPathsFrom(currentPosition, moveRange, heightJump, maxFallHeight);

        // Initialize the current path if available
        if (paths.Count > 0)
        {
            currentPath = null;
        }
        else
        {
            Debug.LogWarning("No paths found from the current position.");
        }

        // Update the rendering of the grid
        UpdateRendering();
    }

    private void OnEnable()
    {
        if (InputReader.Instance == null)
        {
            Debug.LogError("InputReader instance is null. Ensure InputReader is initialized before GridManager.");
            return;
        }

        InputReader.Instance.horizontalEvent += horizontalEvent;
        InputReader.Instance.verticalEvent += verticalEvent;
        InputReader.Instance.confirmEvent += confirmEvent;
        InputReader.Instance.backEvent += backEvent;
    }

    private void OnDisable()
    {
        InputReader.Instance.horizontalEvent -= horizontalEvent;
        InputReader.Instance.verticalEvent -= verticalEvent;
        InputReader.Instance.confirmEvent -= confirmEvent;
        InputReader.Instance.backEvent -= backEvent;
    }

    private void horizontalEvent(int direction)
    {
        if (!isActive)
            return;

        bool foundPath = false;

        newPosition.x += direction;
        if (newPosition.x < 0) newPosition.x = 0;
        if (newPosition.x >= width) newPosition.x = width - 1;
        Debug.Log($"New position after horizontal event: {newPosition}");

        // Check if there's a path to the new position
        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i].destination.gridPosition == newPosition)
            {
                foundPath = true;
                currentPath = paths[i];
                Debug.Log($"Current path updated to: {currentPath.destination.gridPosition}");
                break;
            }
        }

        if (!foundPath)
            currentPath = null; // Reset current path if no valid path found

        // Update the rendering of the grid
        UpdateRendering();
    }

    private void verticalEvent(int direction)
    {
        if (!isActive)
            return;
        
        bool foundPath = false;

        newPosition.y -= direction;
        if (newPosition.y < 0) newPosition.y = 0;
        if (newPosition.y >= height) newPosition.y = height - 1;
        Debug.Log($"New position after vertical event: {newPosition}");

        // Check if there's a path to the new position
        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i].destination.gridPosition == newPosition)
            {
                foundPath = true;
                currentPath = paths[i];
                Debug.Log($"Current path updated to: {currentPath.destination.gridPosition}");
                break;
            }
        }

        if (!foundPath)
            currentPath = null; // Reset current path if no valid path found

        // Update the rendering of the grid
        UpdateRendering();
    }

    private void confirmEvent()
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

    private void backEvent()
    {
        // Handle back event logic here
        Debug.Log("Back event triggered.");
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
                int trueHeight = Random.Range(0, 2); // Random height for demonstration, can be replaced with actual logic

                if (y == 0)
                    trueHeight = x;

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

    public void OnUnitFinishedAction(Unit currentUnit)
    {
        // Reset the new position to the current position after the unit finishes moving
        newPosition = currentUnit.gridPosition;
        Debug.Log($"Unit {currentUnit.name} finished moving. Resetting new position to {newPosition}.");

        // Recalculate paths from the new position
        int moveRange = currentUnit != null ? currentUnit.movementPoints : 5; // Default move range if no unit is set
        int heightJump = currentUnit != null ? currentUnit.jumpHeight : 1; // Default jump height if no unit is set
        int maxFallHeight = currentUnit != null ? currentUnit.maxFallHeight : 10; // Default max fall height if no unit is set

        paths = pathfinding.GetAllPathsFrom(newPosition, moveRange, heightJump, maxFallHeight);
        currentPath = null; // Reset current path
        currentPosition = newPosition; // Update current position to the new position

        // Update the rendering of the grid
        UpdateRendering();

        isActive = true; // Reactivate the grid manager after the unit finishes moving
    }

    private void UpdateRendering()
    {
        foreach (Tile tile in grid)
        {
            if (tile != null)
            {
                if (currentPosition == tile.gridPosition)
                {
                    tile.Illuminate(Color.yellow); // Highlight current position
                }
                else if (newPosition == tile.gridPosition)
                {
                    tile.Illuminate(Color.green); // Highlight new position
                }
                else if (currentPath != null && currentPath.path.Contains(tile))
                {
                    tile.Illuminate(Color.blue); // Highlight path tiles
                }
                else if (paths != null && paths.Count > 0 && paths.Exists(p => p.destination.gridPosition == tile.gridPosition))
                {
                    tile.Illuminate(Color.red); // Highlight destination tiles
                }
                else
                {
                    tile.ResetIllumination(); // Reset other tiles
                }
            }
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