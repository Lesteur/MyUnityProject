using UnityEngine;
using TacticalRPG.Units;

namespace TacticalRPG.Core.States
{
    /// <summary>
    /// State responsible for displaying and handling interactions with the tactical main menu.
    /// </summary>
    public class TacticalStateMainMenu : TacticalStateBase
    {
        /// <summary>
        /// Represents the available main menu actions.
        /// </summary>
        private enum MainMenuAction { Move = 0, Skills = 1, Items = 2, Status = 3, EndTurn = 4 }

        /// <summary>
        /// Initializes a new instance of the <see cref="TacticalStateMainMenu"/> class.
        /// </summary>
        /// <param name="stateMachine">The state machine managing this state.</param>
        public TacticalStateMainMenu(TacticalStateMachine stateMachine) : base(stateMachine) { }

        /// <inheritdoc/>
        public override void Enter(TacticalStateBase previousState)
        {
            Debug.Log("Entering Main Menu State");

            Controller.TacticalMenu.ShowMainMenu(SelectedUnit);

            UpdateRendering();
        }

        /// <inheritdoc/>
        public override void CancelKey()
        {
            if (SelectedUnit == null || SelectedUnit.ActionDone)
                return;

            // If unit moved but hasn’t confirmed
            if (SelectedUnit.MovementDone)
            {
                SelectedUnit.SetPosition(SelectedUnit.PreviousTile.GridPosition);
                SelectedUnit.MovementDone = false;
            }

            _stateMachine.EnterState(_stateMachine.UnitChoiceState);
        }

        /// <inheritdoc/>
        public override void OnClickButton(int buttonIndex)
        {
            switch ((MainMenuAction)buttonIndex)
            {
                case MainMenuAction.Move:
                    _stateMachine.EnterState(_stateMachine.UnitMovementState);
                    break;

                case MainMenuAction.Skills:
                    _stateMachine.EnterState(_stateMachine.SkillMenuState);
                    break;

                case MainMenuAction.Items:
                    Debug.Log("Items menu (not yet implemented).");
                    break;

                case MainMenuAction.Status:
                    Debug.Log("Status screen (not yet implemented).");
                    break;

                case MainMenuAction.EndTurn:
                    _stateMachine.Controller.EndTurn();
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
}