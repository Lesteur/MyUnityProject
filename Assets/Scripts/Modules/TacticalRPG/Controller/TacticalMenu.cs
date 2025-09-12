using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TacticalMenu : MonoBehaviour
{
    [SerializeField] UIDocument tacticalMenuDocument;
    private VisualElement root;

    private Button moveButton;
    private Button skillsButton;
    private Button itemsButton;
    private Button statusButton;
    private Button endTurnButton;

    private void Awake()
    {
        if (tacticalMenuDocument == null)
        {
            Debug.LogError("TacticalMenu: UIDocument reference is missing.");
            return;
        }

        root = tacticalMenuDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("TacticalMenu: Root VisualElement is null.");
            return;
        }

        moveButton = root.Q<Button>("MoveButton");
        skillsButton = root.Q<Button>("SkillsButton");
        itemsButton = root.Q<Button>("ItemsButton");
        statusButton = root.Q<Button>("StatusButton");
        endTurnButton = root.Q<Button>("EndTurnButton");

        // Initially hide the menu
        //root.style.display = DisplayStyle.None;
    }
}
