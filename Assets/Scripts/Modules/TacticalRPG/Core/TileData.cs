using UnityEngine;

namespace TacticalRPG
{
    [CreateAssetMenu(fileName = "New TileData", menuName = "TacticalRPG/TileData")]
    public class TileData : ScriptableObject
    {
        /// <summary>
        /// Enumeration of different terrain types for tiles.
        /// </summary>
        public static enum TerrainType
        {
            Grass,
            Water,
            Mountain,
            Sand,
            Void
        }

        public TerrainType terrainType;
        public Sprite tileSprite;

        public bool isWalkable;
        public int movementCost = 1;
    }
}