using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using Game.Input;

public abstract class TacticalStateBase
{
    protected TacticalStateMachine stateMachine;
    public TacticalController controller => stateMachine.controller;

    public TacticalStateBase(TacticalStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter(TacticalStateBase previousState) { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void PhysicsUpdate() { }
    public virtual void Transition() { }

    public virtual void HorizontalKey(int direction) { }
    public virtual void VerticalKey(int direction) { }
    public virtual void ConfirmKey() { }
    public virtual void CancelKey() { }
}

public class TacticalMainMenuState : TacticalStateBase
{
    public TacticalMainMenuState(TacticalStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the main menu state
        Debug.Log("Entering Main Menu State");
    }

    public override void Update()
    {
        // Logic for updating the main menu state
    }
}

public class TacticalUnitMovementState : TacticalStateBase
{
    public TacticalUnitMovementState(TacticalStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the unit movement state
        Debug.Log("Entering Unit Movement State");
    }

    public override void Update()
    {
        // Logic for updating the unit movement state
    }

    public override void HorizontalKey(int direction)
    {
        controller.MoveCursorTile(direction, 0);
    }

    public override void VerticalKey(int direction)
    {
        controller.MoveCursorTile(0, -direction);
    }

    public override void ConfirmKey()
    {
        controller.MoveUnitPath();
    }

    public override void CancelKey()
    {
        // Handle back event logic here
        Debug.Log("Back event triggered.");
    }
}

public class TacticalUnitActionState : TacticalStateBase
{
    public TacticalUnitActionState(TacticalStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the unit action state
        Debug.Log("Entering Unit Action State");
    }

    public override void Update()
    {
        // Logic for updating the unit action state
    }
}

public class TacticalUnitTargetingState : TacticalStateBase
{
    public TacticalUnitTargetingState(TacticalStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the unit targeting state
        Debug.Log("Entering Unit Targeting State");
    }

    public override void Update()
    {
        // Logic for updating the unit targeting state
    }
}