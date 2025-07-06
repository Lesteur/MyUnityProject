using UnityEngine;

/// <summary> Base class for handle persistent singletons </summary>
/// <typeparam name="T"> The type of the singleton </typeparam>
/// <remarks> Use this class to create a singleton of a scriptable object </remarks>
/// <remarks> This class is used to create a singleton that is not destroyed when loading a new scene </remarks>
public class SingletonPersistent<T> : Singleton<T> where T : SingletonPersistent<T>
{
    /// <summary> Called when the object is created </summary>
    /// <remarks> Set the instance to this object </remarks>
    /// <remarks> This is used to prevent multiple instances of the same object </remarks>
    protected override void Awake()
    {
        base.Awake();

        // Only mark persistent if this is the active instance
        if (instance == this)
            DontDestroyOnLoad(gameObject);

        // Set the name of the object to the type of the singleton
        // This is used to identify the object in the hierarchy
        name = $"[{typeof(T).Name}]";
    }
}