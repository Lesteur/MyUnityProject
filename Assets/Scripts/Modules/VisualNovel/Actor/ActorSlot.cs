using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI container representing a slot where an actor can appear with a body and facial expression.
/// </summary>
public class ActorSlot : MonoBehaviour
{
    public Image bodyImage;
    public Image faceImage;
    private Actor currentActor;

    /// <summary>
    /// Initializes the ActorSlot by finding and assigning the UI images.
    /// /// Ensures the slot is initially hidden.
    /// </summary>
    private void Awake()
    {
        // Initialize references to the UI images
        bodyImage = GetComponent<Image>();
        faceImage = transform.Find("FaceImage").GetComponent<Image>();

        if (bodyImage == null || faceImage == null)
        {
            Debug.LogError("ActorSlot: Required UI images not found in the hierarchy.");
            return;
        }
    }

    /// <summary>
    /// Assigns an actor to this slot and shows default visuals.
    /// </summary>
    /// <param name="actor">Actor to assign.</param>
    public void SetActor(Actor actor)
    {
        currentActor = actor;

        bodyImage.sprite = actor.defaultBody;
        faceImage.sprite = actor.GetMoodSprite("default");
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Changes the actor's facial expression (mood).
    /// </summary>
    /// <param name="moodId">Mood identifier to switch to.</param>
    public void SetMood(string moodId)
    {
        if (currentActor != null)
        {
            var moodSprite = currentActor.GetMoodSprite(moodId);
            if (moodSprite != null)
                faceImage.sprite = moodSprite;
        }
    }

    /// <summary>
    /// Hides the actor's visuals from the scene.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Retrieves the currently assigned actor.
    /// </summary>
    /// <returns>The current actor or null.</returns>
    public Actor GetActor()
    {
        return currentActor;
    }
}