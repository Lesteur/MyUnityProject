using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

using TacticalRPG.Units;
using Utilities;

namespace TacticalRPG.Core
{
    /// <summary>
    /// Enumeration of tactical menu options.
    /// </summary>
    public enum TacticalMenuOptions
    {
        Move,
        Skills,
        Items,
        Status,
        EndTurn,
        Skill0,
        Skill1,
        Skill2,
        Skill3,
        Skill4
    }

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

        public Button MoveButton => _moveButton;
        public Button SkillsButton => _skillsButton;
        public Button ItemsButton => _itemsButton;
        public Button StatusButton => _statusButton;
        public Button EndTurnButton => _endTurnButton;
        public Button[] SkillButtons => _skillButtons;
        public InputAction CancelAction => _cancelAction;

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

            _mainMenu       = _root.Q<VisualElement>("MainMenu");
            _skillMenu      = _root.Q<VisualElement>("SkillMenu");
            _moveButton     = _root.Q<Button>("Move");
            _skillsButton   = _root.Q<Button>("Skills");
            _itemsButton    = _root.Q<Button>("Items");
            _statusButton   = _root.Q<Button>("Status");
            _endTurnButton  = _root.Q<Button>("EndTurn");

            for (int i = 0; i < _skillButtons.Length; i++)
                _skillButtons[i] = _root.Q<Button>($"Skill{i}");

            _root.style.display = DisplayStyle.None;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Shows the main menu for the selected unit.
        /// </summary>
        public void ShowMainMenu(Unit unit)
        {
            if (_root == null || unit == null) return;

            UpdateMainMenu(unit);

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

            SetMenuVisibility(_mainMenu, true);
            SetMenuVisibility(_skillMenu, false);
        }

        public void UpdateMainMenu(Unit unit)
        {
            _moveButton.SetEnabled(!unit.MovementDone);
            _skillsButton.SetEnabled(!unit.ActionDone);

            if (unit.MovementDone && !unit.ActionDone)
            {
                _skillsButton.Focus();
            }
            else if (!unit.MovementDone && unit.ActionDone)
            {
                _moveButton.Focus();
            }
            else if (unit.MovementDone && unit.ActionDone)
            {
                _endTurnButton.Focus();
            }
            else
            {
                _moveButton.Focus();
            }
        }

        /// <summary>
        /// Shows the skill menu for the selected unit.
        /// </summary>
        public void ShowSkillMenu()
        {
            if (_root == null) return;

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