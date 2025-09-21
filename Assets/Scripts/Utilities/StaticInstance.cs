using UnityEngine;

/// <summary> Base class for handle static instances </summary>
/// <typeparam name="T"> The type of the static instance </typeparam>
/// <remarks> Use this class to create a static instance of a scriptable object </remarks>
public abstract class StaticInstance<T> : MonoBehaviour where T : StaticInstance<T>
{
    /// <summary> The static instance of the class </summary>
    public static T Instance { get; private set; }


    /// <summary> Called when the object is created </summary>
    /// <remarks> Set the instance to this object </remarks>
    protected virtual void Awake() => Instance = this as T;


    /// <summary> Called when the object is destroyed </summary>
    /// <remarks> Set the instance to null if it is the same as this object </remarks>
    protected virtual void OnDestroy()
    {
        if (Instance == (this as T))
            Instance = null;
    }
}