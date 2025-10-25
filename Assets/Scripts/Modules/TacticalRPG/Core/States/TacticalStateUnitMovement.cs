using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace TacticalRPG.Core.States
{
    /// <summary>
    /// State responsible for handling unit movement input and rendering path highlights.
    /// </summary>
    public class TacticalStateUnitMovement : TacticalStateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TacticalStateUnitMovement"/> class.
        /// </summary>
        /// <param name="stateMachine">The state machine managing this state.</param>
        public TacticalStateUnitMovement(TacticalStateMachine stateMachine) : base(stateMachine)
        {
            _cursorPosition = Vector2Int.zero;
            _selectedPath = default;
        }

        /// <inheritdoc/>
        public override void Enter(TacticalStateBase previousState)
        {
            Debug.Log("Entering Unit Movement State");

            _cursorPosition = SelectedUnit.GridPosition;
            _selectedPath = default;

            EventSystem.current.SetSelectedGameObject(Controller.gameObject);
            UpdateRendering();
        }

        /// <inheritdoc/>
        public override void HorizontalKey(int direction)
        {
            UpdatePathSelection(new Vector2Int(_cursorPosition.x, _cursorPosition.y - direction));
        }

        /// <inheritdoc/>
        public override void VerticalKey(int direction)
        {
            UpdatePathSelection(new Vector2Int(_cursorPosition.x + direction, _cursorPosition.y));
        }

        /// <inheritdoc/>
        public override void ConfirmKey()
        {
            if (_selectedPath.IsValid)
            {
                _cursorPosition = _selectedPath.Destination.GridPosition;
                Controller.MoveUnitPath(SelectedUnit, _selectedPath);
            }
        }

        /// <inheritdoc/>
        public override void CancelKey()
        {
            _stateMachine.EnterState(_stateMachine.MainMenuState);
        }

        /// <inheritdoc/>
        public override void UpdateRendering()
        {
            if (SelectedUnit == null)
                return;

            RenderCursor();
            RenderTiles();
        }

        /// <summary>
        /// Renders the position of the cursor object in the scene.
        /// </summary>
        private void RenderCursor()
        {
            var tile = Controller.GetTileAt(_cursorPosition);
            if (tile != null)
                Controller.Cursor.transform.position = tile.transform.position;
        }

        /// <summary>
        /// Handles the color illumination logic for all tiles.
        /// </summary>
        private void RenderTiles()
        {
            foreach (Tile tile in Controller.Grid)
            {
                if (tile == null) continue;

                if (SelectedUnit.GridPosition == tile.GridPosition)
                {
                    tile.Illuminate(Color.yellow); // Unit position
                }
                else if (_cursorPosition == tile.GridPosition)
                {
                    tile.Illuminate(Color.green); // Cursor
                }
                else if (_selectedPath.IsValid && _selectedPath.Path.Any(p => p.GridPosition == tile.GridPosition))
                {
                    tile.Illuminate(Color.blue); // Current path
                }
                else if (SelectedUnit.AvailablePaths?.Any(p => p.Destination.GridPosition == tile.GridPosition) == true)
                {
                    tile.Illuminate(Color.red); // Reachable destinations
                }
                else if (tile.TerrainType == TerrainType.Void)
                {
                    tile.Illuminate(Color.gray); // Void terrain
                }
                else
                {
                    tile.ResetIllumination();
                }
            }
        }

        /// <summary>
        /// Updates the currently selected path based on the cursor position.
        /// </summary>
        /// <param name="newPosition">The new cursor position.</param>
        private void UpdatePathSelection(Vector2Int newPosition)
        {
            if (Controller.GetTileAt(newPosition) == null)
                return;

            _cursorPosition = newPosition;

            if (SelectedUnit.AvailablePaths == null)
            {
                _selectedPath = default;
                UpdateRendering();
                return;
            }

            _selectedPath = SelectedUnit.AvailablePaths.FirstOrDefault(
                p => p.Destination.GridPosition == _cursorPosition
            );

            UpdateRendering();
        }

        /// <inheritdoc/>
        public override void OnTileClicked(Tile tile)
        {
            if (tile == null) return;

            UpdatePathSelection(tile.GridPosition);
            ConfirmKey();
        }

        /// <inheritdoc/>
        public override void OnTileHovered(Tile tile)
        {
            if (tile == null) return;

            UpdatePathSelection(tile.GridPosition);
        }
    }
}