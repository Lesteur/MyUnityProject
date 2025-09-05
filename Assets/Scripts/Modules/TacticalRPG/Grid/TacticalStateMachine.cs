using UnityEngine;

public class TacticalStateMachine
{
    public TacticalController controller;
    public TacticalStateBase currentState { get; private set; }

    public TacticalMainMenuState mainMenuState;
    public TacticalUnitMovementState unitMovementState;
    public TacticalUnitActionState unitActionState;
    public TacticalUnitTargetingState unitTargetingState;

    public TacticalStateMachine(TacticalController controller)
    {
        this.controller = controller;

        mainMenuState       = new TacticalMainMenuState(this);
        unitMovementState   = new TacticalUnitMovementState(this);
        unitActionState     = new TacticalUnitActionState(this);
        unitTargetingState  = new TacticalUnitTargetingState(this);

        EnterDefaultState();
    }

    public void Update()
    {
        currentState.Update();
        currentState.Transition();
    }

    public void PhysicsUpdate()
    {
        currentState.PhysicsUpdate();
    }

    public void EnterState(TacticalStateBase newState)
    {
        TacticalStateBase previousState = currentState;

        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(previousState);
    }

    public void EnterDefaultState()
    {
        EnterState(mainMenuState);
    }
}