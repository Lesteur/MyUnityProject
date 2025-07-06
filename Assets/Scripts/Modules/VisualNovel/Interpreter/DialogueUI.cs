using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Defines the available dialogue typing speeds.
/// </summary>
public enum DialogueSpeed
{
    Slow,
    Normal,
    Fast
}

/// <summary>
/// Manages the dialogue user interface, including text display with typing effect,
/// input handling, and sound effects during dialogue.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("UI Components")]
    private GameObject textBox;      // UI container for the dialogue box
    private TMP_Text speakerName;    // UI text element for speaker's name
    private TMP_Text speakerText;    // UI text element for dialogue content

    [Header("Audio")]
    private AudioSource audioSource;      // Source for playing sound effects
    public AudioClip typingSound;         // Sound played when a character appears
    public float pitchVariation = 0.05f;  // Random pitch variation for typing sound

    [Header("Settings")]
    public DialogueSpeed speed = DialogueSpeed.Normal; // Typing speed setting

    private bool isSkipping = false;      // Indicates whether the player is skipping dialogue

    /// <summary>
    /// Unity lifecycle method called when the script instance is being loaded.
    /// Initializes UI components and audio source.
    /// </summary>
    private void Awake()
    {
        textBox = transform.Find("TextBox").gameObject;
        speakerName = textBox.transform.Find("SpeakerName").GetComponent<TMP_Text>();
        speakerText = textBox.transform.Find("SpeakerText").GetComponent<TMP_Text>();

        if (textBox == null)
        {
            Debug.LogError("TextBox GameObject not found in DialogueUI.");
            return;
        }

        if (speakerName == null || speakerText == null)
        {
            Debug.LogError("SpeakerName or SpeakerText components not found in DialogueUI.");
            return;
        }

        // Initialize audio source for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Displays dialogue content with a typing effect and waits for player input to continue.
    /// </summary>
    /// <param name="speakerName">Name of the speaker (can be empty).</param>
    /// <param name="content">The dialogue text to show.</param>
    /// <returns>IEnumerator for coroutine handling.</returns>
    public IEnumerator ShowDialogue(string name, string content)
    {
        if (textBox != null)
            textBox.SetActive(true);

        // Show/hide speaker name based on availability
        speakerName.text = name;
        speakerName.gameObject.SetActive(!string.IsNullOrEmpty(name));

        speakerText.text = "";
        isSkipping = false;

        float delay = GetDelay();

        // Typing effect loop
        for (int i = 0; i < content.Length; i++)
        {
            if (isSkipping)
            {
                speakerText.text = content;
                break;
            }

            speakerText.text += content[i];

            // Play typing sound for non-whitespace characters
            if (!char.IsWhiteSpace(content[i]) && typingSound != null && audioSource != null)
            {
                audioSource.pitch = 1.0f + Random.Range(-pitchVariation, pitchVariation);
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(delay);
        }

        // Wait for user to release input before continuing
        yield return WaitForKeyRelease();

        // Wait for new input to proceed
        yield return WaitForNewInput();
    }

    /// <summary>
    /// Waits for the user to release input keys/buttons before accepting new input.
    /// Prevents accidental skips.
    /// </summary>
    private IEnumerator WaitForKeyRelease()
    {
        while (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            yield return null;
        }
    }

    /// <summary>
    /// Waits for the user to press a key/button to proceed to the next part of dialogue.
    /// </summary>
    private IEnumerator WaitForNewInput()
    {
        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
    }

    /// <summary>
    /// Returns the delay between characters based on the selected typing speed.
    /// </summary>
    /// <returns>Delay in seconds.</returns>
    private float GetDelay()
    {
        return speed switch
        {
            DialogueSpeed.Slow => 0.05f,
            DialogueSpeed.Fast => 0.01f,
            _ => 0.025f,
        };
    }

    /// <summary>
    /// Hides the dialogue box from the UI.
    /// </summary>
    public void HideDialogue()
    {
        if (textBox != null)
            textBox.SetActive(false);
    }

    /// <summary>
    /// Unity update method that listens for user input to skip the dialogue typing animation.
    /// </summary>
    private void Update()
    {
        // Allow skipping only during dialogue display
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            isSkipping = true;
        }
    }
}