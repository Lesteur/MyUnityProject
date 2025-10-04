using UnityEngine;

/// <summary>
/// Manages tactical gameplay states, handling transitions between unit selection,
/// movement, menus, and skill actions.
/// </summary>
public class TacticalStateMachine
{
    /// <summary>
    /// The tactical controller that owns this state machine.
    /// </summary>
    public TacticalController Controller { get; }

    /// <summary>
    /// The currently active tactical state.
    /// </summary>
    public TacticalStateBase CurrentState { get; private set; }

    /// <summary>
    /// State for choosing a unit.
    /// </summary>
    public TacticalStateUnitChoice UnitChoiceState { get; }

    /// <summary>
    /// State for moving a unit.
    /// </summary>
    public TacticalStateUnitMovement UnitMovementState { get; }

    /// <summary>
    /// State for opening and interacting with the main menu.
    /// </summary>
    public TacticalStateMainMenu MainMenuState { get; }

    /// <summary>
    /// State for selecting and executing skills.
    /// </summary>
    public TacticalStateSkillMenu SkillMenuState { get; }

    /// <summary>
    /// State for acting with a unit.
    /// </summary>
    public TacticalStateActingUnit ActingUnitState { get; }
    
    /// <summary>
    /// State for handling the enemy turn.
    /// </summary>
    public TacticalStateEnemyTurn EnemyTurnState { get; }

    /// <summary>
    /// Initializes the tactical state machine and its states.
    /// </summary>
    /// <param name="controller">The tactical controller that owns this state machine.</param>
    public TacticalStateMachine(TacticalController controller)
    {
        Controller = controller;

        UnitChoiceState = new TacticalStateUnitChoice(this);
        UnitMovementState = new TacticalStateUnitMovement(this);
        MainMenuState = new TacticalStateMainMenu(this);
        SkillMenuState = new TacticalStateSkillMenu(this);
        ActingUnitState = new TacticalStateActingUnit(this);
        EnemyTurnState = new TacticalStateEnemyTurn(this);

        EnterDefaultState();
    }

    /// <summary>
    /// Updates the current state and processes transitions.
    /// Should be called once per frame.
    /// </summary>
    public void Update()
    {
        CurrentState.Update();
        CurrentState.Transition();
    }

    /// <summary>
    /// Updates the current state with physics-related logic.
    /// Should be called in FixedUpdate.
    /// </summary>
    public void PhysicsUpdate()
    {
        CurrentState.PhysicsUpdate();
    }

    /// <summary>
    /// Transitions to a new tactical state.
    /// </summary>
    /// <param name="newState">The state to enter.</param>
    public void EnterState(TacticalStateBase newState)
    {
        TacticalStateBase previousState = CurrentState;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter(previousState);
    }

    /// <summary>
    /// Transitions to the default tactical state (unit selection).
    /// </summary>
    public void EnterDefaultState()
    {
        EnterState(UnitChoiceState);
    }
}