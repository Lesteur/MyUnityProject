using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central database for all actors in the game.
/// Uses ScriptableSingleton for easy global access.
/// </summary>
[CreateAssetMenu(fileName = "ActorDatabase", menuName = "VisualNovel/ActorDatabase")]
public class ActorDatabase : ScriptableSingleton<ActorDatabase>
{
    /// <summary>
    /// List of all actor assets to be included in the game.
    /// </summary>
    public List<Actor> allActors;

    /// <summary>
    /// Internal lookup dictionary for quick access by actor ID.
    /// </summary>
    private Dictionary<string, Actor> lookup;

    /// <summary>
    /// Initializes the lookup dictionary with all actors.
    /// </summary>
    public void Initialize()
    {
        lookup = new Dictionary<string, Actor>();
        foreach (var actor in allActors)
        {
            if (!lookup.ContainsKey(actor.id))
            {
                lookup.Add(actor.id, actor);
            }
            else
            {
                Debug.LogWarning($"Duplicate actorId found: {actor.id}");
            }
        }
    }

    /// <summary>
    /// Retrieves an actor by ID from the database.
    /// Initializes the lookup if not already done.
    /// </summary>
    /// <param name="id">Actor's unique identifier.</param>
    /// <returns>The corresponding Actor, or null if not found.</returns>
    public Actor GetActorById(string id)
    {
        if (lookup == null)
            Initialize();

        if (lookup.TryGetValue(id, out var actor))
        {
            return actor;
        }

        Debug.LogError($"Actor with id '{id}' not found.");
        return null;
    }
}