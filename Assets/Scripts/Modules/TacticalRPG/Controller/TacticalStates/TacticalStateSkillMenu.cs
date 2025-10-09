using UnityEngine;

/// <summary>
/// State responsible for displaying the skill menu and handling skill selection.
/// </summary>
public class TacticalStateSkillMenu : TacticalStateBase
{
    private int _selectedIndex = -1;
    private Unit _selectedUnit => TacticalController.Instance.SelectedUnit;
    private SkillData _selectedSkill => TacticalController.Instance.SelectedSkill;

    public TacticalStateSkillMenu(TacticalStateMachine stateMachine) : base(stateMachine) { }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Skill Menu State");

        _selectedIndex = -1;

        TacticalController.Instance.SelectSkill(null);
        TacticalMenu.Instance.ShowSkillMenu();
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void CancelKey()
    {
        // Return to the main menu when cancelling skill selection
        stateMachine.EnterState(stateMachine.MainMenuState);
    }

    /// <inheritdoc/>
    public override void OnClickButton(int buttonIndex)
    {
        if (_selectedUnit == null)
        {
            Debug.LogWarning("No selected unit to choose skill for.");
            TacticalController.Instance.SelectSkill(null);
            return;
        }

        // Validate index
        if (buttonIndex < 0 || buttonIndex >= _selectedUnit.Skills.Count)
        {
            Debug.LogWarning($"Invalid skill index: {buttonIndex}");
            TacticalController.Instance.SelectSkill(null);
            return;
        }

        _selectedIndex = buttonIndex;
        TacticalController.Instance.SelectSkill(_selectedUnit.GetSkillByIndex(buttonIndex));

        Debug.Log($"Skill selected: {_selectedSkill.SkillName.GetLocalizedString()}");

        UpdateRendering();

        // Optional: Transition to targeting state once selection confirmed
        // stateMachine.EnterState(stateMachine.SkillTargetingState);
    }

    /// <inheritdoc/>
    public override void Exit()
    {
        TacticalMenu.Instance.Hide();
        TacticalController.Instance.SelectSkill(null);

        _selectedIndex = -1;
    }

    /// <inheritdoc/>
    public override void UpdateRendering()
    {
        // Reset grid illumination safely
        Controller.ResetAllTiles();

        if (_selectedSkill == null || _selectedUnit == null)
            return;

        // Validate pattern availability
        if (_selectedIndex < 0 || _selectedIndex >= _selectedUnit.MovementPatterns.Count)
        {
            Debug.LogWarning($"No movement pattern found for skill index {_selectedIndex}");
            return;
        }

        var pattern = _selectedUnit.MovementPatterns[_selectedIndex];
        Vector2Int unitPos = _selectedUnit.GridPosition;

        foreach (Vector2Int offset in pattern)
        {
            Tile tile = TacticalController.Instance.GetTileAt(unitPos + offset);
            if (tile != null)
                tile.Illuminate(Color.cyan); // Different highlight color for skill preview
        }
    }
}