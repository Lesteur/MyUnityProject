using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;

public class TacticalStateUnits : TacticalStateBase
{
    private Vector2Int positionCursor;

    public TacticalStateUnits(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        positionCursor = Vector2Int.zero;
    }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the units state
        Debug.Log("Entering Units State");

        UpdateRendering();
    }

    public override void Update()
    {
        // Logic for updating the main menu state
    }

    public override void HorizontalKey(int direction)
    {
        positionCursor.x = Mathf.Clamp(positionCursor.x + direction, 0, controller.width - 1);

        UpdateRendering();
    }

    public override void VerticalKey(int direction)
    {
        positionCursor.y = Mathf.Clamp(positionCursor.y - direction, 0, controller.height - 1);

        UpdateRendering();
    }

    public override void ConfirmKey()
    {
        foreach (Unit unit in controller.units)
        {
            if (unit.gridPosition == positionCursor)
            {
                controller.SelectUnit(unit);
                stateMachine.EnterState(stateMachine.unitActionState);
                return;
            }
        }
    }

    public override void UpdateRendering()
    {
        // Update the rendering of the grid
        foreach (Tile tile in controller.grid)
        {
            if (tile != null)
            {
                if (positionCursor == tile.gridPosition)
                {
                    tile.Illuminate(Color.blue); // Highlight current position
                }
                else
                {
                    tile.ResetIllumination(); // Reset other tiles
                }
            }
        }
    }
}