using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Handles displaying and managing the tactical menu UI, including main actions and skill selections.
/// </summary>
public class TacticalMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument tacticalMenuDocument;
    [SerializeField] private TacticalController controller;

    private VisualElement root;
    private VisualElement mainMenu;
    private VisualElement skillMenu;

    private Button moveButton;
    private Button skillsButton;
    private Button itemsButton;
    private Button statusButton;
    private Button endTurnButton;

    private readonly Button[] skillButtons = new Button[5];

    #region Unity Lifecycle

    private void Awake()
    {
        if (tacticalMenuDocument == null)
        {
            Debug.LogError($"{nameof(TacticalMenu)}: UIDocument reference is missing.");
            return;
        }

        root = tacticalMenuDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError($"{nameof(TacticalMenu)}: Root VisualElement is null.");
            return;
        }

        mainMenu = root.Q<VisualElement>("MainMenu");
        skillMenu = root.Q<VisualElement>("SkillMenu");

        moveButton   = root.Q<Button>("Move");
        skillsButton = root.Q<Button>("Skills");
        itemsButton  = root.Q<Button>("Items");
        statusButton = root.Q<Button>("Status");
        endTurnButton = root.Q<Button>("EndTurn");

        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i] = root.Q<Button>($"Skill{i}");
        }

        // Hide the menu on startup
        root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        moveButton.clicked   += () => OnMainMenuClicked(0);
        skillsButton.clicked += () => OnMainMenuClicked(1);
        itemsButton.clicked  += OnItemsClicked;
        statusButton.clicked += OnStatusClicked;
        endTurnButton.clicked += OnEndTurnClicked;

        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i; // Capture loop variable
            if (skillButtons[i] != null)
            {
                skillButtons[i].clicked += () => OnSkillClicked(index);
            }
        }
    }

    private void OnDisable()
    {
        moveButton.clicked   -= () => OnMainMenuClicked(0);
        skillsButton.clicked -= () => OnMainMenuClicked(1);
        itemsButton.clicked  -= OnItemsClicked;
        statusButton.clicked -= OnStatusClicked;
        endTurnButton.clicked -= OnEndTurnClicked;

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (skillButtons[i] != null)
            {
                skillButtons[i].clicked -= () => OnSkillClicked(i);
            }
        }
    }

    #endregion

    #region Event Handlers

    private void OnMainMenuClicked(int buttonIndex)
    {
        Debug.Log($"Main menu button {buttonIndex} clicked.");
        controller.OnClickButton(buttonIndex);
    }

    private void OnItemsClicked()
    {
        Debug.Log("Items button clicked. (Not yet implemented)");
    }

    private void OnStatusClicked()
    {
        Debug.Log("Status button clicked. (Not yet implemented)");
    }

    private void OnEndTurnClicked()
    {
        Debug.Log("End Turn button clicked. (Not yet implemented)");
    }

    private void OnSkillClicked(int skillIndex)
    {
        Debug.Log($"Skill {skillIndex} button clicked.");
        controller.OnClickButton(skillIndex);
    }

    #endregion

    #region Public API

    /// <summary>
    /// Displays the main action menu.
    /// </summary>
    public void ShowMainMenu()
    {
        if (root == null) return;

        root.style.display = DisplayStyle.Flex;
        mainMenu.style.display = DisplayStyle.Flex;
        skillMenu.style.display = DisplayStyle.None;

        moveButton?.Focus();
    }

    /// <summary>
    /// Displays the skill selection menu.
    /// </summary>
    public void ShowSkillMenu()
    {
        if (root == null) return;

        root.style.display = DisplayStyle.Flex;
        mainMenu.style.display = DisplayStyle.None;
        skillMenu.style.display = DisplayStyle.Flex;

        skillButtons[0]?.Focus();
    }

    /// <summary>
    /// Hides all tactical menus.
    /// </summary>
    public void Hide()
    {
        if (root == null) return;

        root.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.None;
        skillMenu.style.display = DisplayStyle.None;
    }

    #endregion
}