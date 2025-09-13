using UnityEngine;

/// <summary>
/// State responsible for displaying and handling interactions with the tactical main menu.
/// </summary>
public class TacticalStateMainMenu : TacticalStateBase
{
    /// <summary>
    /// Initializes a new instance of the main menu state.
    /// </summary>
    public TacticalStateMainMenu(TacticalStateMachine stateMachine) : base(stateMachine) { }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Main Menu State");
        Controller.TacticalMenu.ShowMainMenu();
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void CancelKey()
    {
        Debug.Log("Back event triggered.");
        stateMachine.EnterState(stateMachine.UnitChoiceState);
    }

    /// <inheritdoc/>
    public override void OnClickButton(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0: // Move
                Debug.Log("Switching to Unit Movement State");
                stateMachine.EnterState(stateMachine.UnitMovementState);
                break;

            case 1: // Skills
                Debug.Log("Switching to Skill Menu State");
                stateMachine.EnterState(stateMachine.SkillMenuState);
                break;

            case 2: // End Turn
                Debug.Log("Ending turn (not yet implemented).");
                // TODO: Add end-turn logic
                break;

            default:
                Debug.LogWarning($"Unhandled button index: {buttonIndex}");
                break;
        }
    }

    /// <inheritdoc/>
    public override void Exit()
    {
        Debug.Log("Exiting Main Menu State");
        Controller.TacticalMenu.Hide();
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