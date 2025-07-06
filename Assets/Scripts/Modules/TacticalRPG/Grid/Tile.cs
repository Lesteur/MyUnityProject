using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TileData tileData;
    public int height;
    public int index;

    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(TileData data, Vector2Int position, int tileHeight, int index)
    {
        tileData = data;
        gridPosition = position;
        height = tileHeight;
        this.index = index;

        gameObject.name = $"Tile_{position.x}_{position.y}_H{height}";
    }

    public bool IsWalkable() => tileData != null && tileData.isWalkable;

    public int GetMovementCost() => tileData?.movementCost ?? 1;

    public void Illuminate(Color color)
    {
        // Example implementation of illumination, can be customized
        GetComponent<SpriteRenderer>().color = color;
    }

    public void ResetIllumination()
    {
        // Reset the tile's color to its original state
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}