using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class SkillArea
{
    public enum AreaType
    {
        Circle,
        Square,
        Cross
    }

    public int minRange;
    public int maxRange;
    public AreaType areaType;

    public List<Tile> GetAffectedTiles(Tile originTile, TacticalController controller)
    {
        List<Tile> affectedTiles = new List<Tile>();
        Vector2Int originPos = originTile.gridPosition;

        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int y = -maxRange; y <= maxRange; y++)
            {
                int distance = Mathf.Abs(x) + Mathf.Abs(y);

                if (areaType == AreaType.Circle && distance > minRange && distance <= maxRange)
                {
                    Tile tile = controller.GetTileAt(new Vector2Int(originPos.x + x, originPos.y + y));
                    if (tile != null)
                        affectedTiles.Add(tile);
                }
                else if (areaType == AreaType.Square && Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) > minRange)
                {
                    Tile tile = controller.GetTileAt(new Vector2Int(originPos.x + x, originPos.y + y));
                    if (tile != null)
                        affectedTiles.Add(tile);
                }
                else if (areaType == AreaType.Cross && (x == 0 || y == 0) && distance > minRange)
                {
                    Tile tile = controller.GetTileAt(new Vector2Int(originPos.x + x, originPos.y + y));
                    if (tile != null)
                        affectedTiles.Add(tile);
                }
            }
        }

        return affectedTiles;
    }
}