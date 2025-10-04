using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// State responsible for selecting a unit on the tactical grid.
/// </summary>
public class TacticalStateUnitChoice : TacticalStateBase
{
    private Vector2Int positionCursor;

    /// <summary>
    /// Creates a new instance of the unit choice state.
    /// </summary>
    public TacticalStateUnitChoice(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        positionCursor = Vector2Int.zero;
    }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Unit Choice State");

        EventSystem.current.SetSelectedGameObject(Controller.gameObject);
        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void Update()
    {
        var hit = GetFocusedOnTile();

        if (hit.HasValue && hit.Value.collider != null)
        {
            var tile = hit.Value.collider.gameObject.GetComponent<Tile>();

            if (tile != null)
                UpdateCursorPosition(tile.GridPosition);
        }
        
        EventSystem.current.SetSelectedGameObject(Controller.gameObject);
    }

    /// <inheritdoc/>
    public override void HorizontalKey(int direction)
    {
        UpdateCursorPosition(new Vector2Int(positionCursor.x, positionCursor.y - direction));
    }

    /// <inheritdoc/>
    public override void VerticalKey(int direction)
    {
        UpdateCursorPosition(new Vector2Int(positionCursor.x + direction, positionCursor.y));
    }

    /// <summary>
    /// Updates the cursor position to the specified grid coordinates.
    /// </summary>
    /// <param name="newPosition">The new grid coordinates for the cursor.</param>
    public void UpdateCursorPosition(Vector2Int newPosition)
    {
        if (TacticalController.Instance.GetTileAt(newPosition) == null)
            return;
        
        positionCursor = newPosition;

        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void ConfirmKey()
    {
        foreach (Unit unit in Controller.AlliedUnits)
        {
            if (unit.GridPosition == positionCursor && !unit.EndTurn)
            {
                Controller.SelectUnit(unit);
                stateMachine.EnterState(stateMachine.MainMenuState);
                return;
            }
        }
    }

    /// <inheritdoc/>
    public override void UpdateRendering()
    {
        Controller.Cursor.transform.position = Controller.Grid[positionCursor.x, positionCursor.y].transform.position;

        foreach (Tile tile in Controller.Grid)
        {
            if (tile == null) continue;

            if (positionCursor == tile.GridPosition)
            {
                tile.Illuminate(Color.blue); // Cursor highlight
            }
            else
            {
                tile.ResetIllumination();
            }
        }
    }

    /// <summary>
    /// Gets the tile currently focused on by the mouse cursor.
    /// </summary>
    private RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        return Physics2D.Raycast(mousePos2D, Vector2.zero);
    }
}