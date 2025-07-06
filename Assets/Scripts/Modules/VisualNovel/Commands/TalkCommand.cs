using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkCommand : IVNCommand
{
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Register()
    {
        CommandRegistry.Register("talk", () => new TalkCommand());
    }
}