using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

using Utilities;
using TacticalRPG.Paths;
using TacticalRPG.Units;
using TacticalRPG.Skills;

namespace TacticalRPG.Core
{
    /// <summary>
    /// Core tactical battle controller.
    /// Handles unit selection, grid generation, turn flow, and input routing.
    /// </summary>
    public class TacticalController : Singleton<TacticalController>,
        IMoveHandler, ISubmitHandler, ICancelHandler
    {
        /// <summary>
        /// Represents the team type in the tactical battle.
        /// </summary>
        private enum Team { Player, Enemy }

        [Header("Grid Settings")]
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private TileData   _defaultTileData;
        [SerializeField] private GameObject _cursor;
        [SerializeField] private Tilemap[]  _tilemaps;

        [Header("Units & UI")]
        [SerializeField] private List<Unit> _allUnits = new();

        private Tile[,] _grid;
        private int     _width;
        private int     _height;

        private Pathfinding             _pathfinding;
        private TacticalStateMachine    _stateMachine;
        private TacticalMenu            _tacticalMenu;

        private Team                _currentTeam    = Team.Player;
        private readonly List<Unit> _alliedUnits    = new();
        private readonly List<Unit> _enemyUnits     = new();

        private Unit        _selectedUnit;
        private SkillData   _selectedSkill;

        #region Events

        /// <summary>
        /// Occurs when a unit becomes the current selection.
        /// </summary>
        public event System.Action<Unit> OnUnitSelectedEvent;

        /// <summary>
        /// Occurs when a skill is selected from the skill menu.
        /// </summary>
        public event System.Action<SkillData> OnSkillSelectedEvent;

        // public event System.Action<Unit> OnUnitActionFinishedEvent;

        /// <summary>
        /// Occurs when the turn passes to a new team.
        /// </summary>
        public event System.Action<int> OnTurnChangedEvent;

        #endregion
        
        #region Properties

        /// <summary> Gets the currently selected unit.</summary>
        public Unit SelectedUnit => _selectedUnit;
        /// <summary> Gets the currently selected skill.</summary>
        public SkillData SelectedSkill => _selectedSkill;
        /// <summary> Gets the tactical grid.</summary>
        public Tile[,] Grid => _grid;
        /// <summary> Gets the grid width.</summary>
        public int Width => _width;
        /// <summary> Gets the grid height.</summary>
        public int Height => _height;
        /// <summary> Gets the cursor GameObject.</summary>
        public GameObject Cursor => _cursor;
        /// <summary> Gets all units in the battle.</summary>
        public List<Unit> AllUnits => _allUnits;
        /// <summary> Gets all allied units.</summary>
        public List<Unit> AlliedUnits => _alliedUnits;
        /// <summary> Gets all enemy units.</summary>
        public List<Unit> EnemyUnits => _enemyUnits;
        /// <summary> Gets the pathfinding component.</summary>
        public Pathfinding Pathfinding => _pathfinding;
        /// <summary> Gets the tactical menu component.</summary>
        public TacticalMenu TacticalMenu => _tacticalMenu;

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// Unity Awake callback. Initializes grid, pathfinding, and state machine.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            GenerateGrid();

            if (_grid == null || _grid.Length == 0)
                Debug.LogError("Grid initialization failed in TacticalController.");

            _tacticalMenu = GetComponent<TacticalMenu>();

            _pathfinding = new Pathfinding(this);
            _stateMachine = new TacticalStateMachine(this);
        }

        /// <summary>
        /// Unity OnEnable callback. Subscribes to tile events.
        /// </summary>
        private void OnEnable()
        {
            Tile.OnTileClicked += (tile) => OnTileClicked(tile);
            Tile.OnTileHovered += (tile) => OnTileHovered(tile);
            Tile.OnTileHoverExited += () => OnTileHoverExited();
        }

        /// <summary>
        /// Unity OnDisable callback. Unsubscribes from tile events.
        /// </summary>
        private void OnDisable()
        {
            Tile.OnTileClicked -= (tile) => OnTileClicked(tile);
            Tile.OnTileHovered -= (tile) => OnTileHovered(tile);
            Tile.OnTileHoverExited -= () => OnTileHoverExited();
        }

        /// <summary>
        /// Unity Start callback. Initializes units and sets starting team.
        /// </summary>
        private void Start()
        {
            InitializeUnits();

            _selectedUnit = null;
            _currentTeam = Team.Player;

            _tacticalMenu.MoveButton.clicked += () => HandleMenuButtonClick(TacticalMenuOptions.Move);
            _tacticalMenu.SkillsButton.clicked += () => HandleMenuButtonClick(TacticalMenuOptions.Skills);
            _tacticalMenu.ItemsButton.clicked += () => HandleMenuButtonClick(TacticalMenuOptions.Items);
            _tacticalMenu.StatusButton.clicked += () => HandleMenuButtonClick(TacticalMenuOptions.Status);
            _tacticalMenu.EndTurnButton.clicked += () => EndTurn();
            _tacticalMenu.CancelAction.performed += ctx => OnCancel(null);

            for (int i = 0; i < _tacticalMenu.SkillButtons.Length; i++)
            {
                TacticalMenuOptions index = (TacticalMenuOptions)(i + (int)TacticalMenuOptions.Skill0);
                _tacticalMenu.SkillButtons[i].clicked += () => HandleMenuButtonClick(index);
            }
        }

        /// <summary>
        /// Unity Update callback. Delegates update to state machine.
        /// </summary>
        private void Update() => _stateMachine?.Update();

        /// <summary>
        /// Unity FixedUpdate callback. Delegates physics update to state machine.
        /// </summary>
        private void FixedUpdate() => _stateMachine?.PhysicsUpdate();

        /// <summary>
        /// Unity OnDestroy callback. Unsubscribes from unit events.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var unit in _allUnits)
            {
                unit.OnMovementComplete -= HandleUnitMovementComplete;
                unit.OnActionComplete -= HandleUnitActionComplete;
            }

            _tacticalMenu.MoveButton.clicked -= () => HandleMenuButtonClick(TacticalMenuOptions.Move);
            _tacticalMenu.SkillsButton.clicked -= () => HandleMenuButtonClick(TacticalMenuOptions.Skills);
            _tacticalMenu.ItemsButton.clicked -= () => HandleMenuButtonClick(TacticalMenuOptions.Items);
            _tacticalMenu.StatusButton.clicked -= () => HandleMenuButtonClick(TacticalMenuOptions.Status);
            _tacticalMenu.EndTurnButton.clicked -= () => EndTurn();
            _tacticalMenu.CancelAction.performed -= ctx => OnCancel(null);

            for (int i = 0; i < _tacticalMenu.SkillButtons.Length; i++)
            {
                TacticalMenuOptions index = (TacticalMenuOptions) (i + (int) TacticalMenuOptions.Skill0);
                _tacticalMenu.SkillButtons[i].clicked -= () => HandleMenuButtonClick(index);
            }
        }

        #endregion
        
        #region Input Handlers

        /// <summary>
        /// Handles directional input for movement.
        /// </summary>
        /// <param name="eventData">Axis event data.</param>
        public void OnMove(AxisEventData eventData)
        {
            Vector2 move = eventData.moveVector;

            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
                _stateMachine.CurrentState.HorizontalKey(move.x > 0 ? 1 : -1);
            else if (Mathf.Abs(move.y) > Mathf.Abs(move.x))
                _stateMachine.CurrentState.VerticalKey(move.y > 0 ? 1 : -1);
        }

        /// <summary>
        /// Handles submit/confirm input.
        /// </summary>
        /// <param name="eventData">Base event data.</param>
        public void OnSubmit(BaseEventData eventData) => _stateMachine.CurrentState.ConfirmKey();

        /// <summary>
        /// Handles cancel input.
        /// </summary>
        /// <param name="eventData">Base event data.</param>
        public void OnCancel(BaseEventData eventData) => _stateMachine.CurrentState.CancelKey();

        /// <summary>
        /// Handles menu button click input.
        /// </summary>
        /// <param name="buttonIndex">Index of the clicked button.</param>
        public void HandleMenuButtonClick(TacticalMenuOptions buttonIndex) => _stateMachine.CurrentState.OnClickButton(buttonIndex);

        #endregion
        
        #region Unit Management

        /// <summary>
        /// Initializes all units, categorizes them by team, and subscribes to their events.
        /// </summary>
        private void InitializeUnits()
        {
            _alliedUnits.Clear();
            _enemyUnits.Clear();

            foreach (var unit in _allUnits)
            {
                unit.Initialize();

                // Subscribe to unit lifecycle events
                unit.OnMovementComplete += HandleUnitMovementComplete;
                unit.OnActionComplete += HandleUnitActionComplete;

                switch (unit.Type)
                {
                    case Unit.UnitType.Player:
                        _alliedUnits.Add(unit);
                        break;
                    case Unit.UnitType.Enemy:
                        _enemyUnits.Add(unit);
                        break;
                }
            }

            foreach (var unit in _allUnits)
                unit.SetAvailablePaths(_pathfinding.GetAllPathsFrom(unit.GridPosition, unit));
        }

        /// <summary>
        /// Selects the given unit and triggers the selection event.
        /// </summary>
        /// <param name="unit">Unit to select.</param>
        public void SelectUnit(Unit unit)
        {
            _selectedUnit = unit;

            if (unit != null)
                OnUnitSelectedEvent?.Invoke(unit);
        }

        /// <summary>
        /// Selects the given skill and triggers the skill selection event.
        /// </summary>
        /// <param name="skill">Skill to select.</param>
        public void SelectSkill(SkillData skill)
        {
            _selectedSkill = skill;

            if (skill != null)
                OnSkillSelectedEvent?.Invoke(skill);
        }

        /// <summary>
        /// Called when a unit finishes its movement phase.
        /// </summary>
        /// <param name="unit">Unit that finished movement.</param>
        private void HandleUnitMovementComplete(Unit unit)
        {
            unit.MovementDone = true;

            // Movement is complete — trigger the next logical phase
            if (unit.Type == Unit.UnitType.Player)
                _stateMachine.EnterState(_stateMachine.MainMenuState);
            else
                EndTurn();
        }

        /// <summary>
        /// Called when a unit completes an action (attack, skill, etc.).
        /// </summary>
        /// <param name="unit">Unit that completed action.</param>
        private void HandleUnitActionComplete(Unit unit)
        {
            unit.ActionDone = true;

            // End the unit’s turn automatically if both actions are complete
            if (unit.MovementDone && unit.ActionDone)
                EndTurn();
            else if (unit.Type == Unit.UnitType.Player)
                _stateMachine.EnterState(_stateMachine.MainMenuState);
        }

        /// <summary>
        /// Moves the unit along the given path.
        /// </summary>
        /// <param name="unit">Unit to move.</param>
        /// <param name="path">Path to follow.</param>
        public void MoveUnitPath(Unit unit, PathResult path)
        {
            if (!path.IsValid) return;

            _stateMachine.EnterState(_stateMachine.ActingUnitState);

            unit.FollowPath(path);
        }

        public void ExecuteSkill(Unit unit, SkillData skill, Vector2Int targetPosition)
        {
            _stateMachine.EnterState(_stateMachine.ActingUnitState);
            
            unit.ExecuteSkill(skill, targetPosition);
        }

        /// <summary>
        /// Resets the turn and action state for the given units.
        /// </summary>
        /// <param name="units">Units to reset.</param>
        private void ResetUnits(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                unit.EndTurn = false;
                unit.MovementDone = false;
                unit.ActionDone = false;
            }
        }

        /// <summary>
        /// Ends the turn for the selected unit and manages team transitions.
        /// </summary>
        public void EndTurn()
        {
            _selectedUnit.EndTurn = true;
            //_selectedUnit = null;

            foreach (var unit in _allUnits)
                unit.SetAvailablePaths(_pathfinding.GetAllPathsFrom(unit.GridPosition, unit));

            switch (_currentTeam)
            {
                case Team.Player:
                    if (_alliedUnits.Exists(u => !u.EndTurn))
                    {
                        _stateMachine.EnterState(_stateMachine.UnitChoiceState);
                        return;
                    }

                    ResetUnits(_enemyUnits);
                    _currentTeam = Team.Enemy;
                    OnTurnChangedEvent?.Invoke((int)Team.Enemy);
                    _stateMachine.EnterState(_stateMachine.EnemyTurnState);
                    break;

                case Team.Enemy:
                    if (_enemyUnits.Exists(u => !u.EndTurn))
                    {
                        _stateMachine.EnterState(_stateMachine.EnemyTurnState);
                        return;
                    }

                    ResetUnits(_alliedUnits);
                    _currentTeam = Team.Player;
                    OnTurnChangedEvent?.Invoke((int)Team.Player);
                    _stateMachine.EnterState(_stateMachine.UnitChoiceState);
                    break;
            }
        }

        #endregion
        
        #region Grid and Tile Management

        /// <summary>
        /// Handles tile click events.
        /// </summary>
        /// <param name="tile">Tile that was clicked.</param>
        private void OnTileClicked(Tile tile) => _stateMachine.CurrentState.OnTileClicked(tile);

        /// <summary>
        /// Handles tile hover events.
        /// </summary>
        /// <param name="tile">Tile that was hovered.</param>
        private void OnTileHovered(Tile tile) => _stateMachine.CurrentState.OnTileHovered(tile);

        /// <summary>
        /// Handles tile hover exit events.
        /// </summary>
        private void OnTileHoverExited() => _stateMachine.CurrentState.OnTileHoverExited();

        /// <summary>
        /// Gets the tile at the specified grid position.
        /// </summary>
        /// <param name="position">Grid position.</param>
        /// <returns>The tile at the position, or null if out of bounds.</returns>
        public Tile GetTileAt(Vector2Int position)
        {
            if (position.x < 0 || position.y < 0 || position.x >= _width || position.y >= _height)
                return null;

            if (_grid == null || _grid.Length == 0)
            {
                Debug.LogError("Grid not initialized.");
                return null;
            }

            return _grid[position.x, position.y];
        }

        /// <summary>
        /// Gets the tile at the specified x and y grid coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>The tile at the coordinates, or null if out of bounds.</returns>
        public Tile GetTileAt(int x, int y) => GetTileAt(new Vector2Int(x, y));

        /// <summary>
        /// Generates the tactical grid from the assigned tilemaps.
        /// </summary>
        private void GenerateGrid()
        {
            if (_tilemaps == null || _tilemaps.Length == 0)
            {
                Debug.LogError("No tilemaps assigned in TacticalController.");
                return;
            }

            Tilemap baseMap = _tilemaps[0];
            BoundsInt bounds = baseMap.cellBounds;

            _width = bounds.size.x;
            _height = bounds.size.y;
            _grid = new Tile[_width, _height];

            int tileCount = 0;

            foreach (Tilemap tilemap in _tilemaps)
            {
                int sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder;

                foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
                {
                    TileBase tileBase = tilemap.GetTile(pos);
                    if (tileBase == null) continue;

                    int x = pos.x - tilemap.cellBounds.xMin;
                    int y = pos.y - tilemap.cellBounds.yMin;
                    int z = pos.z;

                    if (_grid[x, y] != null)
                    {
                        Destroy(_grid[x, y].gameObject);
                        _grid[x, y] = null;
                    }

                    Vector3 worldPos = tilemap.CellToWorld(pos) + new Vector3(0, 0.25f, 0);
                    GameObject tileObj = Instantiate(_tilePrefab, worldPos, Quaternion.identity, transform);
                    tileObj.name = $"Tile_{x}_{y}_H{z}";

                    Tile tile = tileObj.GetComponent<Tile>();
                    tile.Initialize(_defaultTileData, new Vector2Int(x, y), z, sortingOrder);
                    _grid[x, y] = tile;
                    tileCount++;
                }
            }
        }

        /// <summary>
        /// Resets the illumination of all tiles in the grid.
        /// </summary>
        public void ResetAllTiles()
        {
            foreach (var tile in _grid)
            {
                if (tile != null)
                    tile.ResetIllumination();
            }
        }

        #endregion
    }
}