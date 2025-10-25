using UnityEngine;
using UnityEngine.EventSystems;

namespace TacticalRPG.Core.States
{
    /// <summary>
    /// State responsible for targeting with skills on the tactical grid.
    /// </summary>
    public class TacticalStateTargeting : TacticalStateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TacticalStateTargeting"/> class.
        /// </summary>
        /// <param name="stateMachine">The state machine managing this state.</param>
        public TacticalStateTargeting(TacticalStateMachine stateMachine) : base(stateMachine)
        {
            _cursorPosition = Vector2Int.zero;
        }

        /// <inheritdoc/>
        public override void Enter(TacticalStateBase previousState)
        {
            Debug.Log("Entering Targeting State");

            EventSystem.current?.SetSelectedGameObject(Controller.gameObject);
            _lastCursorPosition = _cursorPosition;

            _cursorPosition = SelectedUnit.GridPosition;
            UpdateRendering();
        }

        /// <inheritdoc/>
        public override void HorizontalKey(int direction)
        {
            MoveCursor(Vector2Int.down * direction);
        }

        /// <inheritdoc/>
        public override void VerticalKey(int direction)
        {
            MoveCursor(Vector2Int.right * direction);
        }

        /// <summary>
        /// Moves the cursor by a directional delta, if within grid bounds.
        /// </summary>
        /// <param name="delta">The directional movement vector.</param>
        private void MoveCursor(Vector2Int delta)
        {
            var newPos = _cursorPosition + delta;

            if (Controller.GetTileAt(newPos) != null)
                UpdateCursorPosition(newPos);
        }

        /// <summary>
        /// Updates the cursor position and re-renders highlights.
        /// </summary>
        /// <param name="newPosition">The new cursor position.</param>
        private void UpdateCursorPosition(Vector2Int newPosition)
        {
            if (newPosition == _cursorPosition)
                return;

            _lastCursorPosition = _cursorPosition;
            _cursorPosition = newPosition;

            UpdateRendering();
        }

        /// <inheritdoc/>
        public override void ConfirmKey()
        {
            // Implement skill targeting confirmation logic here if needed.
        }

        /// <inheritdoc/>
        public override void CancelKey()
        {
            _stateMachine.EnterState(_stateMachine.SkillMenuState);
        }

        /// <inheritdoc/>
        public override void UpdateRendering()
        {
            // Reset grid illumination safely
            Controller.ResetAllTiles();

            if (SelectedSkill == null || SelectedUnit == null)
                return;

            var pattern = SelectedUnit.MovementPatterns[SelectedSkill];
            Vector2Int unitPos = SelectedUnit.GridPosition;

            Debug.Log($"Highlighting skill pattern at unit position {unitPos}");

            // Update cursor position
            var lastTile = Controller.GetTileAt(_lastCursorPosition);
            lastTile.ResetIllumination();

            var currentTile = Controller.GetTileAt(_cursorPosition);
            currentTile.Illuminate(Color.blue);

            foreach (Vector2Int offset in pattern)
            {
                Tile tile = Controller.GetTileAt(unitPos + offset);
                if ((tile != null) && (currentTile != tile))
                    tile.Illuminate(Color.cyan); // Different highlight color for skill preview
            }
        }

        /// <inheritdoc/>
        public override void Exit()
        {
            Controller.ResetAllTiles();
        }

        /// <inheritdoc/>
        public override void OnTileClicked(Tile tile)
        {
            if (tile == null) return;

            UpdateCursorPosition(tile.GridPosition);
            ConfirmKey();
        }

        /// <inheritdoc/>
        public override void OnTileHovered(Tile tile)
        {
            if (tile == null) return;

            UpdateCursorPosition(tile.GridPosition);
        }
    }
}