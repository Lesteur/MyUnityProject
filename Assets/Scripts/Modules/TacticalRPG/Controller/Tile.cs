using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a single tile on the tactical grid, including position, terrain, and unit occupancy.
/// </summary>
public class Tile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private TileData tileData;
    [SerializeField] private TerrainType terrainType = TerrainType.Grass; // Default terrain

    public static event System.Action<Tile> OnTileHovered;
    public static event System.Action<Tile> OnTileClicked;
    public static event System.Action OnTileHoverExited;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnTileHovered?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnTileClicked?.Invoke(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnTileHoverExited?.Invoke();
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
    /// The order assigned to this tile in the grid.
    /// </summary>
    public int Order { get; private set; }

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
    public void Initialize(TileData data, Vector2Int position, int tileHeight, int order)
    {
        tileData = data;
        GridPosition = position;
        Height = tileHeight;
        Order = order;
        terrainType = data.terrainType;
        OccupyingUnit = null;

        gameObject.name = $"Tile_{position.x}_{position.y}_H{Height}";
        tileSprite.sortingOrder = order + 1; // Ensure tile is above units

        ResetIllumination();
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