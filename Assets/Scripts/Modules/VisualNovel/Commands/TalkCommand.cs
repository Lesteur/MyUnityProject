using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovel;

/// <summary>
/// Visual novel command that displays dialogue for a specific actor slot.
/// </summary>
public class TalkCommand : IVNCommand
{
    /// <summary>
    /// Executes the talk command asynchronously.
    /// </summary>
    /// <param name="args">List of string arguments passed to the command.</param>
    /// <returns>Coroutine IEnumerator.</returns>
    public IEnumerator Execute(List<string> args)
    {
        if (args.Count < 2)
        {
            Debug.LogError("@talk requires 2 arguments: slotIndex \"Text\"");
            yield break;
        }

        if (!int.TryParse(args[0], out int slotIndex))
        {
            Debug.LogError($"Invalid slotIndex: {args[0]}");
            yield break;
        }

        string text = args[1];
        DialogueManager dialogueManager = GameObject.FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null)
        {
            yield return dialogueManager.ShowDialogue(slotIndex, text);
        }
        else
        {
            Debug.LogError("DialogueManager not found in scene.");
        }
    }

    /// <summary>
    /// Registers the talk command with the command registry before the scene loads.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        CommandRegistry.Register("talk", () => new TalkCommand());
    }
}