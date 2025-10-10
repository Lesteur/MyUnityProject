using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace TacticalRPG
{
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

        /// <summary>
        /// Gets the currently selected unit from the TacticalController.
        /// </summary>
        private Unit SelectedUnit => TacticalController.Instance.SelectedUnit;

        #region Unity Lifecycle

        /// <summary>
        /// Unity Awake callback. Initializes menu UI references and buttons.
        /// </summary>
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

        /// <summary>
        /// Unity OnEnable callback. Subscribes to button and input events.
        /// </summary>
        private void OnEnable()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                Debug.LogError($"{nameof(TacticalMenu)}: No EventSystem found in scene.");
                return;
            }

            var uiModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            _cancelAction = uiModule.cancel.action;
            if (_cancelAction == null)
            {
                Debug.LogError($"{nameof(TacticalMenu)}: Cancel action not found in InputSystemUIInputModule.");
                return;
            }

            // Subscribe
            _moveButton.clicked    += () => OnMainMenuClicked(0);
            _skillsButton.clicked  += () => OnMainMenuClicked(1);
            _itemsButton.clicked   += OnItemsClicked;
            _statusButton.clicked  += OnStatusClicked;
            _endTurnButton.clicked += OnEndTurnClicked;
            _cancelAction.performed += OnCancelPerformed;

            for (int i = 0; i < _skillButtons.Length; i++)
            {
                int index = i;
                if (_skillButtons[i] != null)
                    _skillButtons[i].clicked += () => OnSkillClicked(index);
            }
        }

        /// <summary>
        /// Unity OnDisable callback. Unsubscribes from button and input events.
        /// </summary>
        private void OnDisable()
        {
            if (_cancelAction != null)
                _cancelAction.performed -= OnCancelPerformed;

            _moveButton.clicked    -= () => OnMainMenuClicked(0);
            _skillsButton.clicked  -= () => OnMainMenuClicked(1);
            _itemsButton.clicked   -= OnItemsClicked;
            _statusButton.clicked  -= OnStatusClicked;
            _endTurnButton.clicked -= OnEndTurnClicked;

            for (int i = 0; i < _skillButtons.Length; i++)
            {
                if (_skillButtons[i] != null)
                    _skillButtons[i].clicked -= () => OnSkillClicked(i);
            }
        }

        /// <summary>
        /// Handles cancel input performed event.
        /// </summary>
        /// <param name="ctx">Callback context.</param>
        private void OnCancelPerformed(InputAction.CallbackContext ctx)
        {
            OnCancel();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles main menu button clicks and routes to TacticalController.
        /// </summary>
        /// <param name="index">Index of the clicked button.</param>
        private void OnMainMenuClicked(int index) => TacticalController.Instance.HandleMenuButtonClick(index);

        /// <summary>
        /// Handles the Items button click event.
        /// </summary>
        private void OnItemsClicked() => Debug.Log("Items button clicked. (Not implemented)");

        /// <summary>
        /// Handles the Status button click event.
        /// </summary>
        private void OnStatusClicked() => Debug.Log("Status button clicked. (Not implemented)");

        /// <summary>
        /// Handles the End Turn button click event.
        /// </summary>
        private void OnEndTurnClicked() => TacticalController.Instance.EndTurn();

        /// <summary>
        /// Handles skill button click events and routes to TacticalController.
        /// </summary>
        /// <param name="skillIndex">Index of the clicked skill button.</param>
        private void OnSkillClicked(int skillIndex) => TacticalController.Instance.HandleMenuButtonClick(skillIndex);

        /// <summary>
        /// Handles cancel input and routes to TacticalController.
        /// </summary>
        public void OnCancel() => TacticalController.Instance.OnCancel(null);

        #endregion

        #region Public API

        /// <summary>
        /// Shows the main menu for the selected unit.
        /// </summary>
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

        /// <summary>
        /// Shows the skill menu for the selected unit.
        /// </summary>
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
                    string skillName = skill.SkillName.GetLocalizedString() ?? "Unnamed Skill";

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

            _skillButtons[0].Focus();
        }

        /// <summary>
        /// Hides the tactical menu UI.
        /// </summary>
        public void Hide()
        {
            if (_root == null) return;

            SetMenuVisibility(_mainMenu, false);
            SetMenuVisibility(_skillMenu, false);
            _root.style.display = DisplayStyle.None;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Sets the visibility of a menu VisualElement.
        /// </summary>
        /// <param name="element">The VisualElement to show or hide.</param>
        /// <param name="visible">True to show, false to hide.</param>
        private void SetMenuVisibility(VisualElement element, bool visible)
        {
            if (element == null) return;

            _root.style.display = DisplayStyle.Flex;
            element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        #endregion
    }
}