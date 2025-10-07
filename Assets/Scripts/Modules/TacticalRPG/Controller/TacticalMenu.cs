using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Manages the tactical UI menu for unit actions and skill selections.
/// </summary>
public class TacticalMenu : Singleton<TacticalMenu>
{
    [Header("References")]
    [SerializeField] private UIDocument _menuDocument;

    private VisualElement _root;
    private VisualElement _mainMenu;
    private VisualElement _skillMenu;

    private Button _moveButton;
    private Button _skillsButton;
    private Button _itemsButton;
    private Button _statusButton;
    private Button _endTurnButton;

    private readonly Button[] _skillButtons = new Button[5];
    private InputAction _cancelAction;

    private System.Action _onMoveClicked;
    private System.Action _onSkillsClicked;
    private System.Action _onItemsClicked;
    private System.Action _onStatusClicked;
    private System.Action _onEndTurnClicked;
    private System.Action<InputAction.CallbackContext> _onCancelPerformed;

    private Unit SelectedUnit => TacticalController.Instance.SelectedUnit;

    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();

        if (_menuDocument == null)
        {
            Debug.LogError($"{nameof(TacticalMenu)}: Missing UIDocument reference.");
            return;
        }

        _root = _menuDocument.rootVisualElement;
        if (_root == null)
        {
            Debug.LogError($"{nameof(TacticalMenu)}: Root VisualElement is null.");
            return;
        }

        _mainMenu  = _root.Q<VisualElement>("MainMenu");
        _skillMenu = _root.Q<VisualElement>("SkillMenu");

        _moveButton    = _root.Q<Button>("Move");
        _skillsButton  = _root.Q<Button>("Skills");
        _itemsButton   = _root.Q<Button>("Items");
        _statusButton  = _root.Q<Button>("Status");
        _endTurnButton = _root.Q<Button>("EndTurn");

        for (int i = 0; i < _skillButtons.Length; i++)
            _skillButtons[i] = _root.Q<Button>($"Skill{i}");

        _root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError($"{nameof(TacticalMenu)}: No EventSystem found in scene.");
            return;
        }

        var uiModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        _cancelAction = uiModule.cancel?.action;
        if (_cancelAction == null)
        {
            Debug.LogError($"{nameof(TacticalMenu)}: Cancel action not found in InputSystemUIInputModule.");
            return;
        }

        // Initialize persistent delegates
        _onMoveClicked     = () => OnMainMenuClicked(0);
        _onSkillsClicked   = () => OnMainMenuClicked(1);
        _onItemsClicked    = OnItemsClicked;
        _onStatusClicked   = OnStatusClicked;
        _onEndTurnClicked  = OnEndTurnClicked;
        _onCancelPerformed = ctx => OnCancel();

        // Subscribe
        _moveButton.clicked    += _onMoveClicked;
        _skillsButton.clicked  += _onSkillsClicked;
        _itemsButton.clicked   += _onItemsClicked;
        _statusButton.clicked  += _onStatusClicked;
        _endTurnButton.clicked += _onEndTurnClicked;
        _cancelAction.performed += _onCancelPerformed;

        for (int i = 0; i < _skillButtons.Length; i++)
        {
            int index = i;
            if (_skillButtons[i] != null)
                _skillButtons[i].clicked += () => OnSkillClicked(index);
        }
    }

    private void OnDisable()
    {
        if (_cancelAction != null)
            _cancelAction.performed -= _onCancelPerformed;

        _moveButton.clicked    -= _onMoveClicked;
        _skillsButton.clicked  -= _onSkillsClicked;
        _itemsButton.clicked   -= _onItemsClicked;
        _statusButton.clicked  -= _onStatusClicked;
        _endTurnButton.clicked -= _onEndTurnClicked;

        for (int i = 0; i < _skillButtons.Length; i++)
        {
            if (_skillButtons[i] != null)
                _skillButtons[i].clicked -= () => OnSkillClicked(i);
        }
    }

    #endregion

    #region Event Handlers

    private void OnMainMenuClicked(int index)
        => TacticalController.Instance.HandleMenuButtonClick(index);

    private void OnItemsClicked()
        => Debug.Log("Items button clicked. (Not implemented)");

    private void OnStatusClicked()
        => Debug.Log("Status button clicked. (Not implemented)");

    private void OnEndTurnClicked()
        => TacticalController.Instance.EndTurn();

    private void OnSkillClicked(int skillIndex)
        => TacticalController.Instance.HandleMenuButtonClick(skillIndex);

    public void OnCancel()
        => TacticalController.Instance.OnCancel(null);

    #endregion

    #region Public API

    public void ShowMainMenu()
    {
        if (_root == null || SelectedUnit == null) return;

        SetMenuVisibility(_mainMenu, true);
        SetMenuVisibility(_skillMenu, false);

        if (SelectedUnit.MovementDone)
        {
            _moveButton.SetEnabled(false);
            _skillsButton.Focus();
        }
        else
        {
            _moveButton.SetEnabled(true);
            _moveButton.Focus();
        }
    }

    public void ShowSkillMenu()
    {
        if (_root == null) return;

        var unit = SelectedUnit;
        if (unit == null)
        {
            Debug.LogWarning("No unit selected. Cannot show skill menu.");
            return;
        }

        for (int i = 0; i < _skillButtons.Length; i++)
        {
            var button = _skillButtons[i];
            if (button == null) continue;

            if (i < unit.Skills.Count)
            {
                var skill = unit.Skills[i];
                string skillName = skill?.SkillName.GetLocalizedString() ?? "Unnamed Skill";

                button.text = skillName;
                button.style.display = DisplayStyle.Flex;
                button.SetEnabled(skill != null);
            }
            else
            {
                button.style.display = DisplayStyle.None;
            }
        }

        SetMenuVisibility(_mainMenu, false);
        SetMenuVisibility(_skillMenu, true);

        _skillButtons[0]?.Focus();
    }

    public void Hide()
    {
        if (_root == null) return;

        SetMenuVisibility(_mainMenu, false);
        SetMenuVisibility(_skillMenu, false);
        _root.style.display = DisplayStyle.None;
    }

    #endregion

    #region Helpers

    private void SetMenuVisibility(VisualElement element, bool visible)
    {
        if (element == null) return;

        _root.style.display = DisplayStyle.Flex;
        element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    #endregion
}