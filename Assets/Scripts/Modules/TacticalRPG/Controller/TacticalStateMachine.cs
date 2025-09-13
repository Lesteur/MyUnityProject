using UnityEngine;

public class TacticalStateMachine
{
    public TacticalController controller;
    public TacticalStateBase currentState { get; private set; }

    public TacticalStateUnits unitsState;
    public TacticalStateUnitMovement unitMovementState;
    public TacticalStateUnitActions unitActionState;
    public TacticalStateSkillMenu unitSkillMenuState;
    //public TacticalUnitTargetingState unitTargetingState;

    public TacticalStateMachine(TacticalController controller)
    {
        this.controller = controller;

        unitsState = new TacticalStateUnits(this);
        unitMovementState = new TacticalStateUnitMovement(this);
        unitActionState = new TacticalStateUnitActions(this);
        unitSkillMenuState = new TacticalStateSkillMenu(this);
        //unitTargetingState  = new TacticalStateUnitTargeting(this);

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
        EnterState(unitsState);
    }
}