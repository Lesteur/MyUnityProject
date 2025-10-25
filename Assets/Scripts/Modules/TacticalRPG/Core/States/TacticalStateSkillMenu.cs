using UnityEngine;

namespace TacticalRPG.Core.States
{
    /// <summary>
    /// State responsible for displaying the skill menu and handling skill selection.
    /// </summary>
    public class TacticalStateSkillMenu : TacticalStateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TacticalStateSkillMenu"/> class.
        /// </summary>
        /// <param name="stateMachine">The state machine managing this state.</param>
        public TacticalStateSkillMenu(TacticalStateMachine stateMachine) : base(stateMachine) { }

        /// <inheritdoc/>
        public override void Enter(TacticalStateBase previousState)
        {
            Debug.Log("Entering Skill Menu State");

            Controller.SelectSkill(null);
            TacticalMenu.Instance.ShowSkillMenu();
        }

        /// <inheritdoc/>
        public override void CancelKey()
        {
            // Return to the main menu when cancelling skill selection
            _stateMachine.EnterState(_stateMachine.MainMenuState);
        }

        /// <inheritdoc/>
        public override void OnClickButton(TacticalMenuOptions buttonIndex)
        {
            if (SelectedUnit == null)
            {
                Debug.LogWarning("No selected unit to choose skill for.");
                Controller.SelectSkill(null);
                return;
            }

            // Validate index
            if (buttonIndex < TacticalMenuOptions.Skill0 || buttonIndex > TacticalMenuOptions.Skill4)
            {
                Debug.LogWarning($"Invalid skill index: {buttonIndex}");
                Controller.SelectSkill(null);
                return;
            }

            Controller.SelectSkill(SelectedUnit.GetSkillByIndex((int) buttonIndex - (int) TacticalMenuOptions.Skill0));

            // Transition to targeting state once selection confirmed
            _stateMachine.EnterState(_stateMachine.TargetingState);
        }

        /// <inheritdoc/>
        public override void Exit()
        {
            TacticalMenu.Instance.Hide();
        }
    }
}