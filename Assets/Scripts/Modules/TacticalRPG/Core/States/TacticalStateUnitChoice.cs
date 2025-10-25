using UnityEngine;
using UnityEngine.EventSystems;

namespace TacticalRPG.Core.States
{
    /// <summary>
    /// State responsible for selecting a unit on the tactical grid.
    /// </summary>
    public class TacticalStateUnitChoice : TacticalStateBase
    {
        private Vector2Int _cursorPos;
        private Vector2Int _lastCursorPos;

        /// <summary>
        /// Initializes a new instance of the <see cref="TacticalStateUnitChoice"/> class.
        /// </summary>
        /// <param name="stateMachine">The state machine managing this state.</param>
        public TacticalStateUnitChoice(TacticalStateMachine stateMachine) : base(stateMachine)
        {
            _cursorPos = Vector2Int.zero;
        }

        /// <inheritdoc/>
        public override void Enter(TacticalStateBase previousState)
        {
            Debug.Log("Entering Unit Choice State");

            EventSystem.current.SetSelectedGameObject(Controller.gameObject);
            _lastCursorPos = _cursorPos;

            _cursorPos = Controller.SelectedUnit != null ? Controller.SelectedUnit.GridPosition : Vector2Int.zero;
            Controller.SelectUnit(null);

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
            var newPos = _cursorPos + delta;

            if (Controller.GetTileAt(newPos) != null)
                UpdateCursorPosition(newPos);
        }

        /// <summary>
        /// Updates the cursor position and re-renders highlights.
        /// </summary>
        /// <param name="newPosition">The new cursor position.</param>
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
            foreach (var unit in Controller.AlliedUnits)
            {
                if (unit.GridPosition == _cursorPos && !unit.EndTurn)
                {
                    Controller.SelectUnit(unit);
                    _stateMachine.EnterState(_stateMachine.MainMenuState);
                    return;
                }
            }
        }

        /// <inheritdoc/>
        public override void UpdateRendering()
        {
            var cursor = Controller.Cursor;
            var grid = Controller.Grid;

            // Update cursor position
            cursor.transform.position = grid[_cursorPos.x, _cursorPos.y].transform.position;

            // Reset previous tile highlight
            var lastTile = Controller.GetTileAt(_lastCursorPos);
            lastTile?.ResetIllumination();

            // Highlight current tile
            var currentTile = Controller.GetTileAt(_cursorPos);
            currentTile?.Illuminate(Color.blue);
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