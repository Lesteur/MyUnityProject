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
        controller.tacticalMenu.ShowMainMenu();

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
                Debug.Log("Switching to Unit Movement State");
                stateMachine.EnterState(stateMachine.unitMovementState);
                break;
            case 1: // Skills
                Debug.Log("Switching to Skill Menu State");
                stateMachine.EnterState(stateMachine.unitSkillMenuState);
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
        // Logic for exiting the unit action state
        Debug.Log("Exiting Unit Action State");
        controller.tacticalMenu.Hide();
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