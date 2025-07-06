using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages a list of actor slots, allowing actors to be assigned and their moods to be updated.
/// </summary>
public class ActorManager : MonoBehaviour
{
    /// <summary>
    /// Database containing all available actor definitions.
    /// </summary>
    public ActorDatabase actorDatabase;

    /// <summary>
    /// List of visual slots in the scene where actors can appear.
    /// </summary>
    public List<ActorSlot> slots = new List<ActorSlot>();

    /// <summary>
    /// Assigns an actor to a specific slot by index.
    /// </summary>
    /// <param name="index">Index of the slot to assign the actor to.</param>
    /// <param name="actorId">ID of the actor to assign.</param>
    public void SetActorToSlot(int index, string actorId)
    {
        if (index < 0 || index >= slots.Count)
        {
            Debug.LogError($"Invalid slot index: {index}");
            return;
        }

        Actor data = actorDatabase.GetActorById(actorId);

        if (data != null)
        {
            slots[index].SetActor(data);
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
        if (index < 0 || index >= slots.Count)
        {
            Debug.LogWarning($"Invalid mood change index: {index}");
            return;
        }

        slots[index].SetMood(mood);
    }
}