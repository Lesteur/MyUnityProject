using UnityEngine;

[CreateAssetMenu(fileName = "New MapData", menuName = "TacticalRPG/MapData")]
public class MapData : ScriptableObject
{
    public int width;
    public int height;

    public MapTileData[,] tiles;

    public void Initialize(int w, int h)
    {
        width = w;
        height = h;
        tiles = new MapTileData[w, h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                tiles[x, y] = new MapTileData(null, 0);
            }
        }
    }
}