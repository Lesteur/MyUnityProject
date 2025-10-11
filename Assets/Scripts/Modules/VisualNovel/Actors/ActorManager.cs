using System.Collections.Generic;
using System.Threading.Tasks; // Ajout explicite pour Task
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utilities;

namespace VisualNovel
{
    /// <summary>
    /// Manages a list of actor slots, allowing actors to be assigned and their moods to be updated.
    /// </summary>
    public class ActorManager : Singleton<ActorManager>
    {
        private static Dictionary<string, Actor> _actors = new Dictionary<string, Actor>();

        /// <summary>
        /// List of visual slots in the scene where actors can appear.
        /// </summary>
        [SerializeField] private List<ActorSlot> _slots = new();

        /// <summary>
        /// Gets the list of visual slots in the scene where actors can appear.
        /// </summary>
        public List<ActorSlot> Slots => _slots;

        /// <summary>
        /// Assigns an actor to a specific slot by index.
        /// </summary>
        /// <param name="index">Index of the slot to assign the actor to.</param>
        /// <param name="actorId">ID of the actor to assign.</param>
        public void SetActorToSlot(int index, string actorId)
        {
            if (index < 0 || index >= _slots.Count)
            {
                Debug.LogError($"Invalid slot index: {index}");
                return;
            }

            Actor data = _actors.ContainsKey(actorId) ? _actors[actorId] : null;

            if (data != null)
            {
                _slots[index].SetActor(data);
            }
            else
            {
                Debug.LogWarning($"Actor not found: {actorId}");
            }
        }

        /// <summary>
        /// Sets the mood of an actor in a specific slot.
        /// </summary>
        /// <param name="index">Index of the slot.</param>
        /// <param name="mood">Mood string identifier.</param>
        public void SetActorMood(int index, string mood)
        {
            if (index < 0 || index >= _slots.Count)
            {
                Debug.LogWarning($"Invalid mood change index: {index}");
                return;
            }

            _slots[index].SetMood(mood);
        }

        /// <summary>
        /// Entry point: automatically invoked before the first scene load.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            Debug.Log("Loading actors via Addressables...");
            _actors = new Dictionary<string, Actor>();

            // Start async load
            _ = LoadAllActorsAsync();
        }

        /// <summary>
        /// Asynchronously loads all Actor ScriptableObjects from the Addressables system.
        /// </summary>
        private static async Task LoadAllActorsAsync()
        {
            AsyncOperationHandle<IList<Actor>> handle = Addressables.LoadAssetsAsync<Actor>(
                "Actor", // label passé comme premier argument
                null      // callback optionnel
            );

            try
            {
                IList<Actor> actors = await handle.Task;

                if (handle.Status != AsyncOperationStatus.Succeeded || actors == null)
                {
                    Debug.LogError("Failed to load actors from Addressables.");
                    return;
                }

                foreach (var actor in actors)
                {
                    if (actor == null || string.IsNullOrEmpty(actor.Id))
                    {
                        Debug.LogWarning("Actor asset is null or has empty Id. Skipping.");
                        continue;
                    }
                    if (!_actors.ContainsKey(actor.Id))
                    {
                        _actors.Add(actor.Id, actor);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate actor ID found: {actor.Id}. Skipping.");
                    }
                }

                Debug.Log($"Loaded {_actors.Count} actors.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Exception while loading actors: {ex.Message}");
            }
            finally
            {
                Addressables.Release(handle);
            }
        }
    }
}