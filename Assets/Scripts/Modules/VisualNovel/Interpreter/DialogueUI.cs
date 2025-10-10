using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the dialogue user interface, including text display with typing effect,
/// input handling, and sound effects during dialogue.
/// Uses Unity's new Input System for input handling.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    /// <summary>
    /// Defines the available dialogue typing speeds.
    /// </summary>
    public enum DialogueSpeed
    {
        Slow,
        Normal,
        Fast
    }

    [Header("UI Components")]
    private GameObject _textBox;      // UI container for the dialogue box
    private TMP_Text _speakerName;    // UI text element for speaker's name
    private TMP_Text _speakerText;    // UI text element for dialogue content

    [Header("Audio")]
    private AudioSource _audioSource;      // Source for playing sound effects
    [SerializeField] private AudioClip _typingSound;         // Sound played when a character appears
    [SerializeField] private float _pitchVariation = 0.05f;  // Random pitch variation for typing sound

    [Header("Settings")]
    [SerializeField] public DialogueSpeed Speed = DialogueSpeed.Normal; // Typing speed setting

    private bool _isSkipping = false;      // Indicates whether the player is skipping dialogue

    private InputAction _skipAction;
    private InputAction _continueAction;

    /// <summary>
    /// Unity lifecycle method called when the script instance is being loaded.
    /// Initializes UI components, audio source, and input actions.
    /// </summary>
    private void Awake()
    {
        _textBox = transform.Find("TextBox").gameObject;
        _speakerName = _textBox.transform.Find("SpeakerName").GetComponent<TMP_Text>();
        _speakerText = _textBox.transform.Find("SpeakerText").GetComponent<TMP_Text>();

        if (_textBox == null)
        {
            Debug.LogError("TextBox GameObject not found in DialogueUI.");
            return;
        }

        if (_speakerName == null || _speakerText == null)
        {
            Debug.LogError("SpeakerName or SpeakerText components not found in DialogueUI.");
            return;
        }

        // Initialize audio source for sound effects
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Setup input actions (new Input System)
        _skipAction = new InputAction("SkipDialogue", binding: "<Keyboard>/space");
        _skipAction.AddBinding("<Mouse>/leftButton");
        _continueAction = new InputAction("ContinueDialogue", binding: "<Keyboard>/space");
        _continueAction.AddBinding("<Mouse>/leftButton");

        _skipAction.Enable();
        _continueAction.Enable();
    }

    /// <summary>
    /// Displays dialogue content with a typing effect and waits for player input to continue.
    /// </summary>
    /// <param name="name">Name of the speaker (can be empty).</param>
    /// <param name="content">The dialogue text to show.</param>
    /// <returns>IEnumerator for coroutine handling.</returns>
    public IEnumerator ShowDialogue(string name, string content)
    {
        if (_textBox != null)
            _textBox.SetActive(true);

        // Show/hide speaker name based on availability
        _speakerName.text = name;
        _speakerName.gameObject.SetActive(!string.IsNullOrEmpty(name));

        _speakerText.text = "";
        _isSkipping = false;

        float delay = GetDelay();

        // Typing effect loop
        for (int i = 0; i < content.Length; i++)
        {
            if (_isSkipping)
            {
                _speakerText.text = content;
                break;
            }

            _speakerText.text += content[i];

            // Play typing sound for non-whitespace characters
            if (!char.IsWhiteSpace(content[i]) && _typingSound != null && _audioSource != null)
            {
                _audioSource.pitch = 1.0f + Random.Range(-_pitchVariation, _pitchVariation);
                _audioSource.PlayOneShot(_typingSound);
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
        while (_skipAction.ReadValue<float>() > 0 || _continueAction.ReadValue<float>() > 0)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Waits for the user to press a key/button to proceed to the next part of dialogue.
    /// </summary>
    private IEnumerator WaitForNewInput()
    {
        bool pressed = false;
        while (!pressed)
        {
            if (_continueAction.triggered || _skipAction.triggered)
                pressed = true;
            yield return null;
        }
    }

    /// <summary>
    /// Returns the delay between characters based on the selected typing speed.
    /// </summary>
    /// <returns>Delay in seconds.</returns>
    private float GetDelay()
    {
        return Speed switch
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
        if (_textBox != null)
            _textBox.SetActive(false);
    }

    /// <summary>
    /// Unity update method that listens for user input to skip the dialogue typing animation.
    /// </summary>
    private void Update()
    {
        // Allow skipping only during dialogue display
        if (_skipAction.triggered)
        {
            _isSkipping = true;
        }
    }

    /// <summary>
    /// Unity OnDestroy method to clean up input actions.
    /// </summary>
    private void OnDestroy()
    {
        _skipAction?.Dispose();
        _continueAction?.Dispose();
    }
}