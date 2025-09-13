using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TacticalMenu : MonoBehaviour
{
    [SerializeField] UIDocument tacticalMenuDocument;
    [SerializeField] TacticalController controller;
    private VisualElement root;
    private VisualElement mainMenu;
    private VisualElement skillMenu;

    private Button moveButton;
    private Button skillsButton;
    private Button itemsButton;
    private Button statusButton;
    private Button endTurnButton;

    private Button skill0Button;
    private Button skill1Button;
    private Button skill2Button;
    private Button skill3Button;
    private Button skill4Button;

    private void Awake()
    {
        if (tacticalMenuDocument == null)
        {
            Debug.LogError("TacticalMenu: UIDocument reference is missing.");
            return;
        }

        root = tacticalMenuDocument.rootVisualElement;
        mainMenu = root.Q<VisualElement>("MainMenu");
        skillMenu = root.Q<VisualElement>("SkillMenu");

        if (root == null)
        {
            Debug.LogError("TacticalMenu: Root VisualElement is null.");
            return;
        }

        moveButton = root.Q<Button>("Move");
        skillsButton = root.Q<Button>("Skills");
        itemsButton = root.Q<Button>("Items");
        statusButton = root.Q<Button>("Status");
        endTurnButton = root.Q<Button>("EndTurn");
        skill0Button = root.Q<Button>("Skill0");
        skill1Button = root.Q<Button>("Skill1");
        skill2Button = root.Q<Button>("Skill2");
        skill3Button = root.Q<Button>("Skill3");
        skill4Button = root.Q<Button>("Skill4");

        // Initially hide the menu
        root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        moveButton.clicked += OnMoveClicked;
        skillsButton.clicked += OnSkillsClicked;
        itemsButton.clicked += OnItemsClicked;
        statusButton.clicked += OnStatusClicked;
        endTurnButton.clicked += OnEndTurnClicked;
        skill0Button.clicked += OnSkill0Clicked;
        skill1Button.clicked += OnSkill1Clicked;
        skill2Button.clicked += OnSkill2Clicked;
        skill3Button.clicked += OnSkill3Clicked;
        skill4Button.clicked += OnSkill4Clicked;
    }

    private void OnDisable()
    {
        moveButton.clicked -= OnMoveClicked;
        skillsButton.clicked -= OnSkillsClicked;
        itemsButton.clicked -= OnItemsClicked;
        statusButton.clicked -= OnStatusClicked;
        endTurnButton.clicked -= OnEndTurnClicked;
        skill0Button.clicked -= OnSkill0Clicked;
        skill1Button.clicked -= OnSkill1Clicked;
        skill2Button.clicked -= OnSkill2Clicked;
        skill3Button.clicked -= OnSkill3Clicked;
        skill4Button.clicked -= OnSkill4Clicked;
    }

    private void OnMoveClicked()
    {
        Debug.Log("Move button clicked.");
        // Implement move action

        controller.OnClickButton(0); // Assuming 0 is the index for Move
    }

    private void OnSkillsClicked()
    {
        Debug.Log("Skills button clicked.");
        // Implement skills action

        controller.OnClickButton(1); // Assuming 1 is the index for Skills
    }

    private void OnItemsClicked()
    {
        Debug.Log("Items button clicked.");
        // Implement items action
    }

    private void OnStatusClicked()
    {
        Debug.Log("Status button clicked.");
        // Implement status action
    }

    private void OnEndTurnClicked()
    {
        Debug.Log("End Turn button clicked.");
        // Implement end turn action
    }

    public void OnSkill0Clicked()
    {
        Debug.Log("Skill 0 button clicked.");
        controller.OnClickButton(0); // Assuming 0 is the index for Skill 0
    }

    public void OnSkill1Clicked()
    {
        Debug.Log("Skill 1 button clicked.");
        controller.OnClickButton(1); // Assuming 1 is the index for Skill 1
    }

    public void OnSkill2Clicked()
    {
        Debug.Log("Skill 2 button clicked.");
        controller.OnClickButton(2); // Assuming 2 is the index for Skill 2
    }

    public void OnSkill3Clicked()
    {
        Debug.Log("Skill 3 button clicked.");
        controller.OnClickButton(3); // Assuming 3 is the index for Skill 3
    }

    public void OnSkill4Clicked()
    {
        Debug.Log("Skill 4 button clicked.");
        controller.OnClickButton(4); // Assuming 4 is the index for Skill 4
    }

    public void ShowMainMenu()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.Flex;
            mainMenu.style.display = DisplayStyle.Flex;
            skillMenu.style.display = DisplayStyle.None;

            // Set focus to the move button when showing the menu
            if (moveButton != null)
            {
                moveButton.Focus();
            }
        }
    }

    public void ShowSkillMenu()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.Flex;
            mainMenu.style.display = DisplayStyle.None;
            skillMenu.style.display = DisplayStyle.Flex;

            // Set focus to the first skill button when showing the skill menu
            Button firstSkillButton = skillMenu.Q<Button>("Skill0"); // Assuming buttons are named Skill0, Skill1, etc.
            if (firstSkillButton != null)
            {
                firstSkillButton.Focus();
            }
        }
    }
    
    public void Hide()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.None;
            mainMenu.style.display = DisplayStyle.None;
            skillMenu.style.display = DisplayStyle.None;
        }
    }
}
