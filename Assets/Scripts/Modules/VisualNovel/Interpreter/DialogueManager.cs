using UnityEngine;
using System.Collections;
using UnityEngine.Localization;

namespace VisualNovel
{
    /// <summary>
    /// Manages dialogue display logic, handling actor name resolution and interaction with DialogueUI.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        private DialogueUI _dialogueUI;
        private ActorManager _actorManager;

        /// <summary>
        /// Unity lifecycle method called when the script instance is being loaded.
        /// Initializes references to DialogueUI and ActorManager components in the scene.
        /// </summary>
        private void Awake()
        {
            // Find the DialogueUI component in the scene
            _dialogueUI = GetComponent<DialogueUI>();
            // Find the ActorManager component in the scene
            _actorManager = GetComponent<ActorManager>();
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

            if (_actorManager != null && actorSlotIndex >= 0 && actorSlotIndex < _actorManager.Slots.Count)
            {
                var slot = _actorManager.Slots[actorSlotIndex];
                var actor = slot.GetActor();
                if (actor != null)
                {
                    LocalizedString localizedName = actor.DisplayName;
                    // Convert LocalizedString to a usable string
                    speakerName = localizedName.GetLocalizedString();
                }
            }

            yield return _dialogueUI.ShowDialogue(speakerName, content);
            _dialogueUI.HideDialogue();
        }
    }
}