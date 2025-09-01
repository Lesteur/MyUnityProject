using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TacticalMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private List<Button> buttons;

    private int selectedIndex = 0;

    void Start()
    {
        //Hide();

        Debug.Log("TacticalMenu initialized");
    }

    void Update()
    {
        if (!menuRoot.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            ChangeSelection(1);
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            ChangeSelection(-1);
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            buttons[selectedIndex].onClick.Invoke();
    }

    public void Show()
    {
        menuRoot.SetActive(true);
        selectedIndex = 0;
        UpdateSelectionVisuals();
    }

    public void Hide()
    {
        menuRoot.SetActive(false);
    }

    private void ChangeSelection(int direction)
    {
        selectedIndex += direction;
        if (selectedIndex < 0) selectedIndex = buttons.Count - 1;
        else if (selectedIndex >= buttons.Count) selectedIndex = 0;

        UpdateSelectionVisuals();
    }

    private void UpdateSelectionVisuals()
    {
        EventSystem.current.SetSelectedGameObject(null); // Reset
        EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);
    }

    public void clickAttack()
    {
        // Implement attack logic here
        Debug.Log("Attack button clicked");
    }

    public void ClickMove()
    {
        // Implement move logic here
        Debug.Log("Move button clicked");
    }
    
    public void ClickEndTurn()
    {
        // Implement end turn logic here
        Debug.Log("End Turn button clicked");
    }
}