using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a single tile on the tactical grid, including position, terrain, and unit occupancy.
/// </summary>
public class Tile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private TileData tileData;
    [SerializeField] private TerrainType terrainType = TerrainType.Grass; // Default terrain

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Pointer entered tile at {GridPosition}");
        // Handle pointer enter on tile
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"Left click on tile at {GridPosition}");
            // Handle left click on tile

            TacticalController.Instance.OnPointerClick(eventData);
        }
    }

    /// <summary>
    /// The tile's grid position in the tactical map.
    /// </summary>
    public Vector2Int GridPosition { get; private set; }

    /// <summary>
    /// The vertical height of this tile.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// The index assigned to this tile in the grid.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// The unit currently occupying this tile, if any.
    /// </summary>
    public Unit OccupyingUnit { get; set; }

    /// <summary>
    /// The terrain type assigned to this tile.
    /// </summary>
    public TerrainType TerrainType => terrainType;

    /// <summary>
    /// Initializes this tile with data and grid parameters.
    /// </summary>
    public void Initialize(TileData data, Vector2Int position, int tileHeight, int index)
    {
        tileData        = data;
        GridPosition    = position;
        Height          = tileHeight;
        Index           = index;
        terrainType     = data.terrainType;

        gameObject.name = $"Tile_{position.x}_{position.y}_H{Height}";
    }

    /// <summary>
    /// Determines whether this tile can be walked on.
    /// </summary>
    public bool IsWalkable() =>
        tileData != null && tileData.isWalkable && OccupyingUnit == null;

    /// <summary>
    /// Gets the movement cost for traversing this tile.
    /// </summary>
    public int GetMovementCost() => tileData?.movementCost ?? 1;

    /// <summary>
    /// Highlights this tile with a specified color.
    /// </summary>
    public void Illuminate(Color color)
    {
        tileSprite.enabled = true;
        tileSprite.color = color;
    }

    /// <summary>
    /// Resets the tile's visual highlight to its default state.
    /// </summary>
    public void ResetIllumination()
    {
        tileSprite.enabled = false;
        tileSprite.color = Color.white;
    }
}