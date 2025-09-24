using UnityEngine;

/// <summary>
/// State responsible for displaying and handling interactions with the tactical main menu.
/// </summary>
public class TacticalStateMainMenu : TacticalStateBase
{
    private Unit SelectedUnit => stateMachine.Controller.SelectedUnit;
    private bool startInput = true;

    /// <summary>
    /// Initializes a new instance of the main menu state.
    /// </summary>
    public TacticalStateMainMenu(TacticalStateMachine stateMachine) : base(stateMachine) { }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Main Menu State");

        TacticalMenu.Instance.ShowMainMenu();
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void CancelKey()
    {
        stateMachine.EnterState(stateMachine.UnitChoiceState);
    }

    /// <inheritdoc/>
    public override void OnClickButton(int buttonIndex)
    {
        if (startInput == false)
        {
            startInput = true;
            return; // Ignore the first input to prevent accidental selections
        }

        switch (buttonIndex)
        {
            case 0: // Move
                stateMachine.EnterState(stateMachine.UnitMovementState); break;
            case 1: // Skills
                stateMachine.EnterState(stateMachine.SkillMenuState); break;
            case 2: // End Turn
                Debug.Log("Ending turn (not yet implemented)."); break;
            default:
                Debug.LogWarning($"Unhandled button index: {buttonIndex}"); break;
        }
    }

    /// <inheritdoc/>
    public override void Exit()
    {
        TacticalMenu.Instance.Hide();
    }

    /// <inheritdoc/>
    public override void UpdateRendering()
    {
        foreach (Tile tile in Controller.Grid)
        {
            tile?.ResetIllumination();
        }
    }
}