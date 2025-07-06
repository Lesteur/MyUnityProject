using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActorCommand : IVNCommand, IVNCommandSync
{
    public IEnumerator Execute(List<string> args)
    {
        // This command is intended to be executed immediately, so we can call ExecuteImmediate directly
        ExecuteImmediate(args);
        yield break; // No coroutine needed, so we return immediately
    }

    public void ExecuteImmediate(List<string> args)
    {
        if (args.Count < 2)
        {
            Debug.LogError("setActorSlot requires 2 arguments: slotIndex actorId");
            return;
        }

        if (!int.TryParse(args[0], out int slotIndex))
        {
            Debug.LogError("Invalid slot index: " + args[0]);
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Register()
    {
        CommandRegistry.Register("setActor", () => new SetActorCommand());
    }
}