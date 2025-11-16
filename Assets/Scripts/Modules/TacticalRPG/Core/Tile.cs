using UnityEngine;
using UnityEngine.EventSystems;

using TacticalRPG.Units;

namespace TacticalRPG.Core
{
    /// <summary>
    /// Represents a single tile on the tactical grid, including position, terrain, and unit occupancy.
    /// </summary>
    public class Tile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SpriteRenderer _tileSprite;
        [SerializeField] private TileData _tileData;
        [SerializeField] private TerrainType _terrainType = TerrainType.Grass; // Default terrain

        /// <summary>
        /// Raised when a tile is hovered by the pointer.
        /// </summary>
        public static event System.Action<Tile> OnTileHovered;
        /// <summary>
        /// Raised when a tile is clicked by the pointer.
        /// </summary>
        public static event System.Action<Tile> OnTileClicked;
        /// <summary>
        /// Raised when the pointer exits a tile.
        /// </summary>
        public static event System.Action OnTileHoverExited;

        /// <inheritdoc/>
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnTileHovered.Invoke(this);
        }

        /// <inheritdoc/>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnTileClicked.Invoke(this);
            }
        }

        /// <inheritdoc/>
        public void OnPointerExit(PointerEventData eventData)
        {
            OnTileHoverExited.Invoke();
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
        public TerrainType TerrainType => _terrainType;

        /// <summary>
        /// Initializes this tile with data and grid parameters.
        /// </summary>
        /// <param name="data">The tile data asset.</param>
        /// <param name="position">The grid position of the tile.</param>
        /// <param name="tileHeight">The height of the tile.</param>
        /// <param name="order">The sorting order for rendering.</param>
        public void Initialize(TileData data, Vector2Int position, int tileHeight, int order)
        {
            _tileData       = data;
            GridPosition    = position;
            Height          = tileHeight;
            Order           = order;
            _terrainType    = data.terrainType;
            OccupyingUnit   = null;

            gameObject.name = $"Tile_{position.x}_{position.y}_H{Height}";
            _tileSprite.sortingOrder = order + 1; // Ensure tile is above units

            ResetIllumination();
        }

        /// <summary>
        /// Determines whether this tile can be walked on.
        /// </summary>
        /// <returns>True if walkable and not occupied; otherwise, false.</returns>
        public bool IsWalkable() =>
            _tileData != null && _tileData.isWalkable && OccupyingUnit == null;

        /// <summary>
        /// Gets the movement cost for traversing this tile.
        /// </summary>
        /// <returns>The movement cost value.</returns>
        public int GetMovementCost() => 1;

        /// <summary>
        /// Highlights this tile with a specified color.
        /// </summary>
        /// <param name="color">The color to use for highlighting.</param>
        public void Illuminate(Color color)
        {
            _tileSprite.enabled = true;
            _tileSprite.color = color;
        }

        /// <summary>
        /// Resets the tile's visual highlight to its default state.
        /// </summary>
        public void ResetIllumination()
        {
            _tileSprite.enabled = false;
            _tileSprite.color = Color.white;
        }
    }
}