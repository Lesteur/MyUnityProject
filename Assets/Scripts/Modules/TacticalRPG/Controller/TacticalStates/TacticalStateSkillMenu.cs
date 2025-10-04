using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// State responsible for displaying the skill menu and handling skill selection.
/// </summary>
public class TacticalStateSkillMenu : TacticalStateBase
{
    private int index = 0;
    private SkillData selectedSkill;
    private Unit selectedUnit => Controller.SelectedUnit;

    /// <summary>
    /// Initializes a new instance of the skill menu state.
    /// </summary>
    public TacticalStateSkillMenu(TacticalStateMachine stateMachine) : base(stateMachine) { }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Skill Menu State");

        selectedSkill = null;
        TacticalMenu.Instance.ShowSkillMenu();

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
        
        index = buttonIndex;

        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void Exit()
    {
        TacticalMenu.Instance.Hide();
        selectedSkill = null;
    }

    /// <inheritdoc/>
    public override void UpdateRendering()
    {
        // Reset all tiles first
        foreach (Tile tile in Controller.Grid)
            tile?.ResetIllumination();

        if (selectedSkill == null || selectedUnit == null) return;

        // Highlight tiles affected by the selected skill
        List<Tile> affectedTiles = new List<Tile>();
        List<Vector2Int> pattern = selectedUnit.MovementPatterns[index];
        Vector2Int unitPos = selectedUnit.GridPosition;

        foreach (Vector2Int pos in pattern)
        {
            Tile tile = TacticalController.Instance.GetTileAt(pos + unitPos);
            
            if (tile != null)
                affectedTiles.Add(tile);
        }

        foreach (Tile tile in affectedTiles)
            tile?.Illuminate(Color.blue); // Example: highlight affected area
    }
}