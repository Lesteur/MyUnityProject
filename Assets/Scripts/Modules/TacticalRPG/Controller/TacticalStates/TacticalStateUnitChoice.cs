using UnityEngine;

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
            {
                UpdateCursorPosition(tile.GridPosition);
            }
        }
    }

    /// <inheritdoc/>
    public override void HorizontalKey(int direction)
    {
        UpdateCursorPosition(new Vector2Int(Mathf.Clamp(
            positionCursor.x + direction,
            0,
            Controller.Grid.GetLength(0) - 1), positionCursor.y));
    }

    /// <inheritdoc/>
    public override void VerticalKey(int direction)
    {
        UpdateCursorPosition(new Vector2Int(positionCursor.x, Mathf.Clamp(
            positionCursor.y - direction,
            0,
            Controller.Grid.GetLength(1) - 1)));
    }

    /// <summary>
    /// Updates the cursor position to the specified grid coordinates.
    /// </summary>
    /// <param name="newPosition">The new grid coordinates for the cursor.</param>
    public void UpdateCursorPosition(Vector2Int newPosition)
    {
        positionCursor = newPosition;

        Controller.Cursor.transform.position = Controller.Grid[positionCursor.x, positionCursor.y].transform.position + new Vector3(0, 0.25f, 0);
        Controller.Cursor.GetComponent<SpriteRenderer>().sortingOrder = Controller.Grid[positionCursor.x, positionCursor.y].GetComponent<SpriteRenderer>().sortingOrder;

        UpdateRendering();
    }

    /// <inheritdoc/>
    public override void ConfirmKey()
    {
        foreach (Unit unit in Controller.Units)
        {
            if (unit.GridPosition == positionCursor)
            {
                Controller.SelectUnit(unit);
                stateMachine.EnterState(stateMachine.MainMenuState);
                return;
            }
        }

        Debug.Log("No unit found at cursor position.");
    }

    /// <inheritdoc/>
    public override void UpdateRendering()
    {
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