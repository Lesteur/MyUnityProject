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
    private bool pathFound;

    /// <summary>
    /// Initializes a new instance of the unit movement state.
    /// </summary>
    public TacticalStateUnitMovement(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        positionCursor = Vector2Int.zero;
        selectedPath = null;
    }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Unit Movement State");

        positionCursor = SelectedUnit.GridPosition;
        selectedPath = null;
        pathFound = false;

        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void HorizontalKey(int direction)
    {
        positionCursor.x = Mathf.Clamp(positionCursor.x + direction, 0, stateMachine.Controller.Grid.GetLength(0) - 1);
        UpdatePathSelection();
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void VerticalKey(int direction)
    {
        positionCursor.y = Mathf.Clamp(positionCursor.y - direction, 0, stateMachine.Controller.Grid.GetLength(1) - 1);
        UpdatePathSelection();
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void ConfirmKey()
    {
        if (pathFound)
        {
            Debug.Log($"Path confirmed to {selectedPath.destination.gridPosition}.");
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

            if (SelectedUnit != null && SelectedUnit.GridPosition == tile.gridPosition)
            {
                tile.Illuminate(Color.yellow); // Current position
            }
            else if (positionCursor == tile.gridPosition)
            {
                tile.Illuminate(Color.green); // Cursor position
            }
            else if (selectedPath != null && selectedPath.path.Exists(t => t.gridPosition == tile.gridPosition))
            {
                tile.Illuminate(Color.blue); // Path tiles
            }
            else if (SelectedUnit?.AvailablePaths != null && SelectedUnit.AvailablePaths.Exists(p => p.destination.gridPosition == tile.gridPosition))
            {
                tile.Illuminate(Color.red); // Reachable destinations
            }
            else if (tile.terrainType == TerrainType.Void)
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
            selectedPath = null;
            pathFound = false;
            return;
        }

        selectedPath = SelectedUnit.AvailablePaths.Find(p => p.destination.gridPosition == positionCursor);
        pathFound = selectedPath != null;
    }
}