using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;

public class TacticalStateUnitActions : TacticalStateBase
{
    public TacticalStateUnitActions(TacticalStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the unit action state
        Debug.Log("Entering Unit Action State");

        controller.actionMenu.Show(0);

        UpdateRendering();
    }

    public override void Update()
    {
        // Logic for updating the unit action state
    }

    public override void CancelKey()
    {
        // Handle back event logic here
        Debug.Log("Back event triggered.");

        stateMachine.EnterState(stateMachine.unitsState);
    }

    public override void OnClickButton(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0: // Move
                stateMachine.EnterState(stateMachine.unitMovementState);
                break;
            case 1: // Attack
                // Implement attack logic here
                break;
            case 2: // End Turn
                // Implement end turn logic here
                break;
            default:
                Debug.LogWarning("Unhandled button index: " + buttonIndex);
                break;
        }
    }

    public override void Exit()
    {
        base.Exit();
        controller.actionMenu.Hide();
    }

    public override void UpdateRendering()
    {
        foreach (Tile tile in controller.grid)
        {
            if (tile != null)
            {
                tile.ResetIllumination();
            }
        }
    }
}