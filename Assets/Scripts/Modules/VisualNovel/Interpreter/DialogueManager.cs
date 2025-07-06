using UnityEngine;
using System.Collections;
using UnityEngine.Localization;

/// <summary>
/// Manages dialogue display logic, handling actor name resolution and interaction with DialogueUI.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    private DialogueUI dialogueUI;
    private ActorManager actorManager;

    /// <summary>
    /// Unity lifecycle method called when the script instance is being loaded.
    /// Initializes references to DialogueUI and ActorManager components in the scene.
    /// </summary>
    void Awake()
    {
        // Find the DialogueUI component in the scene
        dialogueUI = GetComponent<DialogueUI>();
        // Find the ActorManager component in the scene
        actorManager = GetComponent<ActorManager>();
    }

    /// <summary>
    /// Shows dialogue for a specific actor slot.
    /// </summary>
    /// <param name="actorSlotIndex">Index of the actor slot to speak.</param>
    /// <param name="content">Text content to display.</param>
    /// <returns>Coroutine enumerator.</returns>
    public IEnumerator ShowDialogue(int actorSlotIndex, string content)
    {
        string speakerName = "";

        if (actorSlotIndex >= 0 && actorSlotIndex < actorManager.slots.Count)
        {
            var slot = actorManager.slots[actorSlotIndex];
            LocalizedString localizedName = slot.GetActor().displayName;

            // Convert LocalizedString to a usable string
            speakerName = localizedName.GetLocalizedString();
        }

        yield return dialogueUI.ShowDialogue(speakerName, content);
        dialogueUI.HideDialogue();
    }
}