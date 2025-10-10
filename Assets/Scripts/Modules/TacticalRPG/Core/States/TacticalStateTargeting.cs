using UnityEngine;
using UnityEngine.EventSystems;

namespace TacticalRPG
{
    /// <summary>
    /// State responsible for selecting a unit on the tactical grid.
    /// </summary>
    public class TacticalStateTargeting : TacticalStateBase
    {
        private Vector2Int _cursorPos;
        private Vector2Int _lastCursorPos;
        private Unit _selectedUnit => TacticalController.Instance.SelectedUnit;
        private SkillData _selectedSkill => TacticalController.Instance.SelectedSkill;

        public TacticalStateTargeting(TacticalStateMachine stateMachine) : base(stateMachine)
        {
            _cursorPos = Vector2Int.zero;
        }

        /// <inheritdoc/>
        public override void Enter(TacticalStateBase previousState)
        {
            Debug.Log("Entering Targeting State");

            EventSystem.current?.SetSelectedGameObject(Controller.gameObject);
            _lastCursorPos = _cursorPos;

            UpdateRendering();
        }

        /// <inheritdoc/>
        public override void HorizontalKey(int direction) => MoveCursor(Vector2Int.down * direction);

        /// <inheritdoc/>
        public override void VerticalKey(int direction) => MoveCursor(Vector2Int.right * direction);

        /// <summary>
        /// Moves the cursor by a directional delta, if within grid bounds.
        /// </summary>
        private void MoveCursor(Vector2Int delta)
        {
            var newPos = _cursorPos + delta;

            if (Controller.GetTileAt(newPos) != null)
                UpdateCursorPosition(newPos);
        }

        /// <summary>
        /// Updates the cursor position and re-renders highlights.
        /// </summary>
        private void UpdateCursorPosition(Vector2Int newPosition)
        {
            if (newPosition == _cursorPos)
                return;

            _lastCursorPos = _cursorPos;
            _cursorPos = newPosition;

            UpdateRendering();
        }

        /// <inheritdoc/>
        public override void ConfirmKey()
        {
            /*
            foreach (var unit in Controller.AlliedUnits)
            {
                if (unit.GridPosition == _cursorPos && !unit.EndTurn)
                {
                    Controller.SelectUnit(unit);
                    stateMachine.EnterState(stateMachine.MainMenuState);
                    return;
                }
            }
            */
        }

        public override void CancelKey() => stateMachine.EnterState(stateMachine.SkillMenuState);

        /// <inheritdoc/>
        public override void UpdateRendering()
        {
            // Reset grid illumination safely
            Controller.ResetAllTiles();

            if (_selectedSkill == null || _selectedUnit == null)
                return;

            var pattern = _selectedUnit.MovementPatterns[_selectedSkill];
            Vector2Int unitPos = _selectedUnit.GridPosition;

            Debug.Log($"Highlighting skill pattern at unit position {unitPos}");

            // Update cursor position
            var lastTile = Controller.GetTileAt(_lastCursorPos);
            lastTile.ResetIllumination();

            var currentTile = Controller.GetTileAt(_cursorPos);
            currentTile.Illuminate(Color.blue);

            foreach (Vector2Int offset in pattern)
            {
                Tile tile = TacticalController.Instance.GetTileAt(unitPos + offset);
                if ((tile != null) && (currentTile != tile))
                    tile.Illuminate(Color.cyan); // Different highlight color for skill preview
            }
        }

        public override void Exit()
        {
            Controller.ResetAllTiles();
        }

        public override void OnTileClicked(Tile tile)
        {
            if (tile == null) return;

            UpdateCursorPosition(tile.GridPosition);
            ConfirmKey();
        }

        public override void OnTileHovered(Tile tile)
        {
            if (tile == null) return;

            UpdateCursorPosition(tile.GridPosition);
        }
    }
}