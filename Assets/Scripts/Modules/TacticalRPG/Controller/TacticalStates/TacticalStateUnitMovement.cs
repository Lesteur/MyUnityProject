using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;

public class TacticalStateUnitMovement : TacticalStateBase
{
    private Unit selectedUnit => controller.selectedUnit;
    private Vector2Int positionCursor;
    private PathResult selectedPath;
    private bool pathFound;

    public TacticalStateUnitMovement(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        positionCursor = Vector2Int.zero;
        selectedPath = null;
    }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the unit movement state
        Debug.Log("Entering Unit Movement State");

        positionCursor = selectedUnit.gridPosition;
        selectedPath = null;
        pathFound = false;

        UpdateRendering();
    }

    public override void Update()
    {
        // Logic for updating the unit movement state
    }

    public override void HorizontalKey(int direction)
    {
        positionCursor.x = Mathf.Clamp(positionCursor.x + direction, 0, controller.width - 1);

        if (selectedUnit != null && selectedUnit.availablePaths != null)
        {
            selectedPath = selectedUnit.availablePaths.Find(p => p.destination.gridPosition == positionCursor);
            pathFound = selectedPath != null;
        }

        UpdateRendering();
    }

    public override void VerticalKey(int direction)
    {
        positionCursor.y = Mathf.Clamp(positionCursor.y - direction, 0, controller.height - 1);

        if (selectedUnit != null && selectedUnit.availablePaths != null)
        {
            selectedPath = selectedUnit.availablePaths.Find(p => p.destination.gridPosition == positionCursor);
            pathFound = selectedPath != null;
        }

        UpdateRendering();
    }

    public override void ConfirmKey()
    {
        if (pathFound)
        {
            Debug.Log($"Path confirmed to {selectedPath.destination.gridPosition}.");

            controller.MoveUnitPath(selectedUnit, selectedPath);
        }
        else
        {
            Debug.Log("No valid path found to the selected position.");
        }
    }

    public override void CancelKey()
    {
        // Handle back event logic here
        Debug.Log("Back event triggered.");

        stateMachine.EnterState(stateMachine.unitActionState);
    }

    public override void UpdateRendering()
    {
        foreach (Tile tile in controller.grid)
        {
            if (tile != null)
            {
                if (selectedUnit != null && selectedUnit.gridPosition == tile.gridPosition)
                {
                    tile.Illuminate(Color.yellow); // Highlight current position
                }
                else if (positionCursor == tile.gridPosition)
                {
                    tile.Illuminate(Color.green); // Highlight new position
                }
                else if (selectedPath != null && selectedPath.path.Exists(t => t.gridPosition == tile.gridPosition))
                {
                    tile.Illuminate(Color.blue); // Highlight path tiles
                }
                else if (selectedUnit != null && selectedUnit.availablePaths != null && selectedUnit.availablePaths.Exists(p => p.destination.gridPosition == tile.gridPosition))
                {
                    tile.Illuminate(Color.red); // Highlight destination tiles
                }
                else if (tile.terrainType == TerrainType.Void)
                {
                    tile.Illuminate(Color.grey); // Highlight occupied tiles
                }
                else
                {
                    tile.ResetIllumination(); // Reset other tiles
                }
            }
        }
    }
}