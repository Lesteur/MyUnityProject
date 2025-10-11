using UnityEngine;
using System.IO;

namespace Utilities
{
    /// <summary> Base class for creating a globally accessible singleton of a ScriptableObject </summary>
    /// <typeparam name="T"> The type of the singleton </typeparam>
    /// <remarks> Use this class to create a singleton of a scriptable object </remarks>
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _instance;

        /// <summary> Returns the singleton instance of the ScriptableObject </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Try to load from the Resources folder
                    string typeName = typeof(T).Name;
                    _instance = Resources.Load<T>(typeName);

#if UNITY_EDITOR
                    // If not found, create a new asset (Editor only)
                    if (_instance == null)
                    {
                        _instance = CreateInstance<T>();

                        string folderPath = "Assets/Resources";
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        string assetPath = $"{folderPath}/{typeName}.asset";
                        UnityEditor.AssetDatabase.CreateAsset(_instance, assetPath);
                        UnityEditor.AssetDatabase.SaveAssets();

                        Debug.LogWarning($"[ScriptableSingleton] Created new instance of {typeName} at {assetPath}");
                    }
#endif

                    if (_instance == null)
                        Debug.LogError($"[ScriptableSingleton] Could not load {typeName} from Resources.");
                }

                return _instance;
            }
        }
    }
}