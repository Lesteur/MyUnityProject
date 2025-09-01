using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Unit : MonoBehaviour
{
    public Vector2Int gridPosition; // The position of the unit in the grid
    public int movementPoints; // The number of movement points the unit has
    public int jumpHeight = 1; // The maximum height the unit can jump
    public int maxFallHeight = 10; // The maximum height the unit can fall

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private Pathfinding pathfinding; // Reference to the Pathfinding component
    private TacticalController tacticalController; // Reference to the TacticalController component
    private bool isMoving = false; // Flag to indicate if the unit is currently moving
    private PathResult path;
    private Tile currentTile;

    private void Awake()
    {
        // Get the Pathfinding component from the TacticalController
        pathfinding = GetComponent<Pathfinding>();
        // Get the SpriteRenderer component from the GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get the TacticalController component from the scene
        tacticalController = FindFirstObjectByType<TacticalController>();

        currentTile = tacticalController.GetTileAt(gridPosition);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float x = currentTile.transform.position.x;
        float y = currentTile.transform.position.y; // Adjust height based on tile height
        transform.position = new Vector3(x, y + 0.4f, 0);

        spriteRenderer.sortingOrder = currentTile.GetComponent<SpriteRenderer>().sortingOrder + 1; // Set sorting order based on tile
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void GetPath(PathResult pathResult)
    {
        if (pathResult == null || pathResult.path == null || pathResult.path.Count == 0)
        {
            Debug.LogError("Path is empty or null.");
            return;
        }

        // Set the path and start moving
        path = pathResult;
        StartCoroutine(MoveAlongPath());
    }

    public bool isUnitMoving()
    {
        return isMoving;
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        // Iterate through each tile in the path
        foreach (Tile tile in path.path)
        {
            // Calculate the target position based on the tile's position
            Vector3 targetPosition = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.4f, 0);

            // Move towards the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 3f);
                yield return null; // Wait for the next frame
            }

            // Update the unit's grid position to the current tile's grid position
            transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.4f, 0);
            spriteRenderer.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1; // Update sorting order based on tile
            gridPosition = tile.gridPosition;
        }

        isMoving = false;

        // Notify the TacticalController that the unit has finished moving
        if (tacticalController != null)
        {
            tacticalController.OnUnitFinishedAction(this);
        }
        else
        {
            Debug.LogError("TacticalController reference is null.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(new Vector3(gridPosition.x, -gridPosition.y, 0), 0.25f);
    }
}
