using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// State responsible for handling unit movement input and rendering path highlights.
/// </summary>
public class TacticalStateUnitMovement : TacticalStateBase
{
    private Unit SelectedUnit => stateMachine.Controller.SelectedUnit;
    private Vector2Int positionCursor;
    private PathResult selectedPath;

    /// <summary>
    /// Initializes a new instance of the unit movement state.
    /// </summary>
    public TacticalStateUnitMovement(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        positionCursor = Vector2Int.zero;
        selectedPath = default; // struct default (invalid state)
    }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Unit Movement State");

        positionCursor = SelectedUnit.GridPosition;
        selectedPath = default;

        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void Update()
    {
        var hit = GetFocusedOnTile();

        if (hit.HasValue && hit.Value.collider != null)
        {
            var tile = hit.Value.collider.gameObject.GetComponent<Tile>();
            if (tile != null)
            {
                positionCursor = tile.GridPosition;

                Controller.Cursor.transform.position = Controller.Grid[positionCursor.x, positionCursor.y].transform.position + new Vector3(0, 0.25f, 0);
                Controller.Cursor.GetComponent<SpriteRenderer>().sortingOrder = Controller.Grid[positionCursor.x, positionCursor.y].GetComponent<SpriteRenderer>().sortingOrder;

                UpdatePathSelection();
                UpdateRendering();
            }
        }
    }

    /// <inheritdoc/>
    public override void HorizontalKey(int direction)
    {
        positionCursor.x = Mathf.Clamp(
            positionCursor.x + direction,
            0,
            stateMachine.Controller.Grid.GetLength(0) - 1
        );

        Controller.Cursor.transform.position = Controller.Grid[positionCursor.x, positionCursor.y].transform.position + new Vector3(0, 0.25f, 0);
        Controller.Cursor.GetComponent<SpriteRenderer>().sortingOrder = Controller.Grid[positionCursor.x, positionCursor.y].GetComponent<SpriteRenderer>().sortingOrder;

        UpdatePathSelection();
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void VerticalKey(int direction)
    {
        positionCursor.y = Mathf.Clamp(
            positionCursor.y - direction,
            0,
            stateMachine.Controller.Grid.GetLength(1) - 1
        );

        Controller.Cursor.transform.position = Controller.Grid[positionCursor.x, positionCursor.y].transform.position + new Vector3(0, 0.25f, 0);
        Controller.Cursor.GetComponent<SpriteRenderer>().sortingOrder = Controller.Grid[positionCursor.x, positionCursor.y].GetComponent<SpriteRenderer>().sortingOrder;

        UpdatePathSelection();
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void ConfirmKey()
    {
        if (selectedPath.IsValid)
        {
            Debug.Log($"Path confirmed to {selectedPath.Destination.GridPosition}.");
            stateMachine.Controller.MoveUnitPath(SelectedUnit, selectedPath);
        }
        else
        {
            Debug.Log("No valid path found to the selected position.");
        }
    }

    /// <inheritdoc/>
    public override void CancelKey()
    {
        Debug.Log("Back event triggered.");
        stateMachine.EnterState(stateMachine.MainMenuState);
    }

    /// <inheritdoc/>
    public override void UpdateRendering()
    {
        foreach (Tile tile in stateMachine.Controller.Grid)
        {
            if (tile == null) continue;

            if (SelectedUnit != null && SelectedUnit.GridPosition == tile.GridPosition)
            {
                tile.Illuminate(Color.yellow); // Current unit position
            }
            else if (positionCursor == tile.GridPosition)
            {
                tile.Illuminate(Color.green); // Cursor position
            }
            else if (selectedPath.IsValid && ContainsTile(selectedPath.Path, tile))
            {
                tile.Illuminate(Color.blue); // Path tiles
            }
            else if (SelectedUnit?.AvailablePaths != null &&
                     ContainsDestination(SelectedUnit.AvailablePaths, tile))
            {
                tile.Illuminate(Color.red); // Reachable destinations
            }
            else if (tile.TerrainType == TerrainType.Void)
            {
                tile.Illuminate(Color.gray); // Void tiles
            }
            else
            {
                tile.ResetIllumination(); // Default
            }
        }
    }

    /// <summary>
    /// Updates the currently selected path based on the cursor position.
    /// </summary>
    private void UpdatePathSelection()
    {
        if (SelectedUnit?.AvailablePaths == null)
        {
            selectedPath = default;
            return;
        }

        foreach (var path in SelectedUnit.AvailablePaths)
        {
            if (path.Destination.GridPosition == positionCursor)
            {
                selectedPath = path;
                return;
            }
        }

        selectedPath = default; // No match found
    }

    /// <summary>
    /// Checks if a tile exists in a given path.
    /// </summary>
    private static bool ContainsTile(IReadOnlyList<Tile> path, Tile tile)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (path[i].GridPosition == tile.GridPosition)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if any available path leads to the specified tile.
    /// </summary>
    private static bool ContainsDestination(List<PathResult> paths, Tile tile)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i].Destination.GridPosition == tile.GridPosition)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the tile currently focused on by the mouse cursor.
    /// </summary>
    private RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        return Physics2D.Raycast(mousePos2D, Vector2.zero);
    }
}