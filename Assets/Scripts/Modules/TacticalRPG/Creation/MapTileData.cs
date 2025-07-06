using UnityEngine;

[System.Serializable]
public class MapTileData
{
    public int height;
    public TileData tileData;

    public MapTileData(TileData data, int height)
    {
        this.tileData = data;
        this.height = height;
    }
}