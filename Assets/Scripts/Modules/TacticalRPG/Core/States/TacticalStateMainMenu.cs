using UnityEngine;

namespace TacticalRPG.Core.States
{
    /// <summary>
    /// State responsible for displaying and handling interactions with the tactical main menu.
    /// </summary>
    public class TacticalStateMainMenu : TacticalStateBase
    {
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
        public override void OnClickButton(TacticalMenuOptions buttonIndex)
        {
            switch (buttonIndex)
            {
                case TacticalMenuOptions.Move:
                    _stateMachine.EnterState(_stateMachine.UnitMovementState);
                    break;

                case TacticalMenuOptions.Skills:
                    _stateMachine.EnterState(_stateMachine.SkillMenuState);
                    break;

                case TacticalMenuOptions.Items:
                    Debug.Log("Items menu (not yet implemented).");
                    break;

                case TacticalMenuOptions.Status:
                    Debug.Log("Status screen (not yet implemented).");
                    break;

                case TacticalMenuOptions.EndTurn:
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