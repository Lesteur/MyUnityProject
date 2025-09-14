using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the area of effect pattern for a skill, including range and shape.
/// </summary>
[System.Serializable]
public class SkillArea
{
    /// <summary>
    /// The geometric pattern used to calculate affected tiles.
    /// </summary>
    private enum AreaType
    {
        Circle,
        Square,
        Cross
    }

    [SerializeField, Min(0)] private int minRange = 0;
    [SerializeField, Min(1)] private int maxRange = 1;
    [SerializeField] private AreaType areaType = AreaType.Circle;

    /// <summary> Minimum distance from the origin tile that tiles can be affected. </summary>
    public int MinRange => minRange;

    /// <summary> Maximum distance from the origin tile that tiles can be affected. </summary>
    public int MaxRange => maxRange;

    /// <summary> The shape of the skill's area of effect. </summary>
    //public AreaType Shape => areaType;

    /// <summary>
    /// Returns a list of tiles affected by this area pattern.
    /// </summary>
    /// <param name="originTile">The tile where the effect originates (usually the caster's position).</param>
    /// <param name="controller">Reference to the tactical controller for grid access.</param>
    public List<Tile> GetAffectedTiles(Tile originTile, TacticalController controller)
    {
        List<Tile> affectedTiles = new List<Tile>();
        Vector2Int originPos = originTile.GridPosition;

        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int y = -maxRange; y <= maxRange; y++)
            {
                int distance = Mathf.Abs(x) + Mathf.Abs(y);
                Vector2Int testPos = new Vector2Int(originPos.x + x, originPos.y + y);

                if (IsTileInArea(x, y, distance))
                {
                    Tile tile = controller.GetTileAt(testPos);
                    if (tile != null)
                        affectedTiles.Add(tile);
                }
            }
        }

        return affectedTiles;
    }

    /// <summary>
    /// Checks whether a given offset is included in the current area type.
    /// </summary>
    private bool IsTileInArea(int x, int y, int distance)
    {
        switch (areaType)
        {
            case AreaType.Circle:
                return distance > minRange && distance <= maxRange;
            case AreaType.Square:
                return Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) > minRange && 
                       Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) <= maxRange;
            case AreaType.Cross:
                return (x == 0 || y == 0) && distance > minRange && distance <= maxRange;
            default:
                return false;
        }
    }
}