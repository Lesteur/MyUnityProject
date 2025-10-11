using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovel;

namespace Cutscenes
{
    /// <summary>
    /// Visual novel command that assigns an actor to a specific slot.
    /// Implements both asynchronous and synchronous command interfaces.
    /// </summary>
    public class SetActorSlotCommand : IVNCommand, IVNCommandSync
    {
        /// <summary>
        /// Executes the set actor slot command asynchronously.
        /// </summary>
        /// <param name="args">List of string arguments passed to the command.</param>
        /// <returns>Coroutine IEnumerator.</returns>
        public IEnumerator Execute(List<string> args)
        {
            ExecuteImmediate(args);
            yield break; // No coroutine needed, so we return immediately
        }

        /// <summary>
        /// Executes the set actor slot command synchronously.
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

            string actorId = args[1];
            ActorManager actorManager = GameObject.FindFirstObjectByType<ActorManager>();
            if (actorManager != null)
            {
                actorManager.SetActorToSlot(slotIndex, actorId);
            }
            else
            {
                Debug.LogError("ActorManager not found in scene.");
            }
        }

        /// <summary>
        /// Registers the set actor slot command with the command registry before the scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            CommandRegistry.Register("setActorSlot", () => new SetActorSlotCommand());
        }
    }
}