using UnityEngine;

/// <summary>
/// State responsible for displaying and handling interactions with the tactical main menu.
/// </summary>
public class TacticalStateMainMenu : TacticalStateBase
{
    private enum MainMenuAction { Move = 0, Skills = 1, Items = 2, Status = 3, EndTurn = 4 }

    private Unit _selectedUnit => stateMachine.Controller.SelectedUnit;

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
        if (_selectedUnit == null || _selectedUnit.ActionDone)
            return;

        // If unit moved but hasn’t confirmed
        if (_selectedUnit.MovementDone)
        {
            _selectedUnit.SetPosition(_selectedUnit.PreviousTile.GridPosition);
            _selectedUnit.MovementDone = false;
        }

        stateMachine.EnterState(stateMachine.UnitChoiceState);
    }

    /// <inheritdoc/>
    public override void OnClickButton(int buttonIndex)
    {
        switch ((MainMenuAction)buttonIndex)
        {
            case MainMenuAction.Move:
                stateMachine.EnterState(stateMachine.UnitMovementState);
                break;

            case MainMenuAction.Skills:
                stateMachine.EnterState(stateMachine.SkillMenuState);
                break;

            case MainMenuAction.Items:
                Debug.Log("Items menu (not yet implemented).");
                break;

            case MainMenuAction.Status:
                Debug.Log("Status screen (not yet implemented).");
                break;

            case MainMenuAction.EndTurn:
                stateMachine.Controller.EndTurn();
                break;

            default:
                Debug.LogWarning($"Unhandled menu button index: {buttonIndex}");
                break;
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
        Controller.ResetAllTiles();
    }
}