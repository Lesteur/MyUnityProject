using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// State responsible for displaying the skill menu and handling skill selection.
/// </summary>
public class TacticalStateSkillMenu : TacticalStateBase
{
    private SkillData selectedSkill;

    /// <summary>
    /// Initializes a new instance of the skill menu state.
    /// </summary>
    public TacticalStateSkillMenu(TacticalStateMachine stateMachine) : base(stateMachine) { }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Skill Menu State");

        selectedSkill = null;
        Controller.TacticalMenu.ShowSkillMenu();
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void CancelKey()
    {
        stateMachine.EnterState(stateMachine.MainMenuState);
    }

    /// <inheritdoc/>
    public override void OnClickButton(int buttonIndex)
    {
        if (Controller.SelectedUnit == null)
        {
            selectedSkill = null;
            return;
        }

        if (buttonIndex >= 0 && buttonIndex < Controller.SelectedUnit.Skills.Count)
            selectedSkill = Controller.SelectedUnit.GetSkillByIndex(buttonIndex);
        else
            selectedSkill = null;

        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void Exit()
    {
        Controller.TacticalMenu.Hide();
    }

    /// <inheritdoc/>
    public override void UpdateRendering()
    {
        // Reset all tiles first
        foreach (Tile tile in Controller.Grid)
            tile?.ResetIllumination();

        if (selectedSkill == null || Controller.SelectedUnit == null) return;

        // Highlight tiles affected by the selected skill
        List<Tile> affectedTiles = selectedSkill.AreaOfEffect.GetAffectedTiles(
            Controller.SelectedUnit.CurrentTile,
            Controller);

        foreach (Tile tile in affectedTiles)
            tile?.Illuminate(Color.blue); // Example: highlight affected area
    }
}