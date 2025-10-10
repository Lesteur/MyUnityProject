using UnityEngine;

namespace TacticalRPG
{
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
            // UpdateRendering();
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

            // Optional: Transition to targeting state once selection confirmed
            stateMachine.EnterState(stateMachine.TargetingState);
        }

        /// <inheritdoc/>
        public override void Exit()
        {
            TacticalMenu.Instance.Hide();

            _selectedIndex = -1;
        }
    }
}