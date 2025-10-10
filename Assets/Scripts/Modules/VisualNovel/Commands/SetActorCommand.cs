using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovel;

/// <summary>
/// Visual novel command that sets the mood of an actor in a specific slot.
/// Implements both asynchronous and synchronous command interfaces.
/// </summary>
public class SetActorCommand : IVNCommand, IVNCommandSync
{
    /// <summary>
    /// Executes the set actor command asynchronously.
    /// </summary>
    /// <param name="args">List of string arguments passed to the command.</param>
    /// <returns>Coroutine IEnumerator.</returns>
    public IEnumerator Execute(List<string> args)
    {
        ExecuteImmediate(args);
        yield break; // No coroutine needed, so we return immediately
    }

    /// <summary>
    /// Executes the set actor command synchronously.
    /// </summary>
    /// <param name="args">List of string arguments passed to the command.</param>
    public void ExecuteImmediate(List<string> args)
    {
        if (args.Count < 2)
        {
            Debug.LogError("setActorSlot requires 2 arguments: slotIndex actorId");
            return;
        }

        if (!int.TryParse(args[0], out int slotIndex))
        {
            Debug.LogError($"Invalid slot index: {args[0]}");
            return;
        }

        string mood = args[1];
        ActorManager actorManager = GameObject.FindFirstObjectByType<ActorManager>();
        if (actorManager != null)
        {
            actorManager.SetActorMood(slotIndex, mood);
        }
        else
        {
            Debug.LogError("ActorManager not found in scene.");
        }
    }

    /// <summary>
    /// Registers the set actor command with the command registry before the scene loads.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        CommandRegistry.Register("setActor", () => new SetActorCommand());
    }
}