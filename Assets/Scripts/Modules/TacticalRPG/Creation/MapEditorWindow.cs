using UnityEditor;
using UnityEngine;

public class MapEditorWindow : EditorWindow
{
    private MapData mapData;
    private TileData selectedTileData;
    private int height = 0;
    private Vector2 scroll;
    private int width = 10;
    private int heightMap = 10;

    [MenuItem("TacticalRPG/Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<MapEditorWindow>("Map Editor");
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        mapData = (MapData)EditorGUILayout.ObjectField("Map Data", mapData, typeof(MapData), false);
        selectedTileData = (TileData)EditorGUILayout.ObjectField("Terrain Type", selectedTileData, typeof(TileData), false);
        height = EditorGUILayout.IntField("Height", height);

        EditorGUILayout.Space();
        width = EditorGUILayout.IntField("Grid Width", width);
        heightMap = EditorGUILayout.IntField("Grid Height", heightMap);

        if (GUILayout.Button("Create / Reset Grid"))
        {
            if (mapData != null)
            {
                mapData.Initialize(width, heightMap);
                EditorUtility.SetDirty(mapData);
            }
        }

        if (mapData == null || mapData.tiles == null)
            return;

        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int y = mapData.height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < mapData.width; x++)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.fixedWidth = 40;
                style.fixedHeight = 40;

                string label = mapData.tiles[x, y].tileData ? mapData.tiles[x, y].tileData.terrainType.ToString().Substring(0, 1) : ".";
                label += "\n" + mapData.tiles[x, y].height;

                if (GUILayout.Button(label, style))
                {
                    mapData.tiles[x, y].tileData = selectedTileData;
                    mapData.tiles[x, y].height = height;
                    EditorUtility.SetDirty(mapData);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}