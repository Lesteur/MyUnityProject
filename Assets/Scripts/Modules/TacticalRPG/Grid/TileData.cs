using UnityEngine;

public enum TerrainType
{
    Grass,
    Water,
    Mountain,
    Sand,
    Void
}

[CreateAssetMenu(fileName = "New TileData", menuName = "TacticalRPG/TileData")]
public class TileData : ScriptableObject
{
    public TerrainType terrainType;
    public Sprite tileSprite;

    public bool isWalkable;
    public int movementCost = 1;
}