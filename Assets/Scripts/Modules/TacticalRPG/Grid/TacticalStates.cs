using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using Game.Input;

public abstract class TacticalStateBase
{
    protected TacticalStateMachine stateMachine;
    public TacticalController controller => stateMachine.controller;

    public TacticalStateBase(TacticalStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter(TacticalStateBase previousState) { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void PhysicsUpdate() { }
    public virtual void Transition() { }
    public virtual void UpdateRendering() { }

    public virtual void HorizontalKey(int direction) { }
    public virtual void VerticalKey(int direction) { }
    public virtual void ConfirmKey() { }
    public virtual void CancelKey() { }

    public virtual void OnClickButton(int buttonIndex) { }
}

public class TacticalMainMenuState : TacticalStateBase
{
    Vector2Int positionCursor;

    public TacticalMainMenuState(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        positionCursor = Vector2Int.zero;
    }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the main menu state
        Debug.Log("Entering Main Menu State");

        positionCursor = controller.currentPosition;

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
        if (positionCursor == controller.currentPosition)
            stateMachine.EnterState(stateMachine.unitActionState);
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

public class TacticalUnitMovementState : TacticalStateBase
{
    public Vector2Int currentPosition;
    public Vector2Int newPosition;

    public TacticalUnitMovementState(TacticalStateMachine stateMachine) : base(stateMachine)
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

        stateMachine.EnterState(stateMachine.unitActionState);
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

public class TacticalUnitActionState : TacticalStateBase
{
    public TacticalUnitActionState(TacticalStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the unit action state
        Debug.Log("Entering Unit Action State");
    }

    public override void Update()
    {
        // Logic for updating the unit action state
    }

    public override void CancelKey()
    {
        // Handle back event logic here
        Debug.Log("Back event triggered.");

        stateMachine.EnterState(stateMachine.mainMenuState);
    }
}

public class TacticalUnitTargetingState : TacticalStateBase
{
    public TacticalUnitTargetingState(TacticalStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the unit targeting state
        Debug.Log("Entering Unit Targeting State");
    }

    public override void Update()
    {
        // Logic for updating the unit targeting state
    }
}