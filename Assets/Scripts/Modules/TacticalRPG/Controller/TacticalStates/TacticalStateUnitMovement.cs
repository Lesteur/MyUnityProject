using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

/// <summary>
/// State responsible for handling unit movement input and rendering path highlights.
/// </summary>
public class TacticalStateUnitMovement : TacticalStateBase
{
    private Unit _selectedUnit => stateMachine.Controller.SelectedUnit;
    private Vector2Int _cursorPosition;
    private PathResult _selectedPath;

    public TacticalStateUnitMovement(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        _cursorPosition = Vector2Int.zero;
        _selectedPath = default;
    }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Unit Movement State");

        _cursorPosition = _selectedUnit.GridPosition;
        _selectedPath = default;

        EventSystem.current.SetSelectedGameObject(Controller.gameObject);
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void HorizontalKey(int direction) =>
        UpdatePathSelection(new Vector2Int(_cursorPosition.x, _cursorPosition.y - direction));

    /// <inheritdoc/>
    public override void VerticalKey(int direction) =>
        UpdatePathSelection(new Vector2Int(_cursorPosition.x + direction, _cursorPosition.y));

    /// <inheritdoc/>
    public override void ConfirmKey()
    {
        if (_selectedPath.IsValid)
            Controller.MoveUnitPath(_selectedUnit, _selectedPath);
    }

    /// <inheritdoc/>
    public override void CancelKey()
    {
        stateMachine.EnterState(stateMachine.MainMenuState);
    }

    /// <inheritdoc/>
    public override void UpdateRendering()
    {
        if (_selectedUnit == null)
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

            if (_selectedUnit.GridPosition == tile.GridPosition)
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
            else if (_selectedUnit.AvailablePaths?.Any(p => p.Destination.GridPosition == tile.GridPosition) == true)
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
    private void UpdatePathSelection(Vector2Int newPosition)
    {
        if (Controller.GetTileAt(newPosition) == null)
            return;

        _cursorPosition = newPosition;

        if (_selectedUnit?.AvailablePaths == null)
        {
            _selectedPath = default;
            UpdateRendering();
            return;
        }

        _selectedPath = _selectedUnit.AvailablePaths.FirstOrDefault(
            p => p.Destination.GridPosition == _cursorPosition
        );

        UpdateRendering();
    }

    public override void OnTileClicked(Tile tile)
    {
        if (tile == null) return;

        UpdatePathSelection(tile.GridPosition);
        ConfirmKey();
    }

    public override void OnTileHovered(Tile tile)
    {
        if (tile == null) return;

        UpdatePathSelection(tile.GridPosition);
    }
}