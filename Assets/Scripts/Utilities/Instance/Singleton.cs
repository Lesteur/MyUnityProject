namespace Utilities
{
    /// <summary> Base class for handle singletons </summary>
    /// <typeparam name="T"> The type of the singleton </typeparam>
    /// <remarks> Use this class to create a singleton of a scriptable object </remarks>
    public abstract class Singleton<T> : StaticInstance<T> where T : Singleton<T>
    {
        /// <summary> Called when the object is created </summary>
        /// <remarks> Set the instance to this object </remarks>
        /// <remarks> This is used to prevent multiple instances of the same object </remarks>
        protected override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();
        }
    }
}