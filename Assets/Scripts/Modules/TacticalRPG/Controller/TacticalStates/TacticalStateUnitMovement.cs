using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;

public class TacticalStateUnitMovement : TacticalStateBase
{
    public Vector2Int currentPosition;
    public Vector2Int newPosition;

    public TacticalStateUnitMovement(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        currentPosition = Vector2Int.zero;
        newPosition     = Vector2Int.zero;
    }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the unit movement state
        Debug.Log("Entering Unit Movement State");

        UpdateRendering();
    }

    public override void Update()
    {
        // Logic for updating the unit movement state
    }

    public override void HorizontalKey(int direction)
    {
        controller.MoveCursorTile(direction, 0);

        UpdateRendering();
    }

    public override void VerticalKey(int direction)
    {
        controller.MoveCursorTile(0, -direction);

        UpdateRendering();
    }

    public override void ConfirmKey()
    {
        controller.MoveUnitPath();
    }

    public override void CancelKey()
    {
        // Handle back event logic here
        Debug.Log("Back event triggered.");

        stateMachine.EnterState(stateMachine.unitsState);
    }

    public override void UpdateRendering()
    {
        foreach (Tile tile in controller.grid)
        {
            if (tile != null)
            {
                if (controller.currentPosition == tile.gridPosition)
                {
                    tile.Illuminate(Color.yellow); // Highlight current position
                }
                else if (controller.newPosition == tile.gridPosition)
                {
                    tile.Illuminate(Color.green); // Highlight new position
                }
                else if (controller.currentPath != null && controller.currentPath.path.Contains(tile))
                {
                    tile.Illuminate(Color.blue); // Highlight path tiles
                }
                else if (controller.paths != null && controller.paths.Count > 0 && controller.paths.Exists(p => p.destination.gridPosition == tile.gridPosition))
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