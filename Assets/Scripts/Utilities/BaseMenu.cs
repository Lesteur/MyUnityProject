using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BaseMenu : MonoBehaviour
{
    public List<Button> buttons;

    private int selectedIndex = 0;

    void Awake()
    {
        if (buttons == null || buttons.Count == 0)
        {
            Debug.LogWarning("No buttons assigned to the menu.");
        }

        Hide();

        Debug.Log("Menu initialized");
    }

    public void Show(int index)
    {
        if (buttons.Count == 0)
        {
            Debug.LogWarning("No buttons assigned to the menu.");
            return;
        }

        selectedIndex = Mathf.Clamp(index, 0, buttons.Count - 1);
        EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}