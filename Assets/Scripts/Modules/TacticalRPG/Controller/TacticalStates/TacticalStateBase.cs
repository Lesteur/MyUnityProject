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
    public virtual void UpdateRendering() { }

    public virtual void HorizontalKey(int direction) { }
    public virtual void VerticalKey(int direction) { }
    public virtual void ConfirmKey() { }
    public virtual void CancelKey() { }

    public virtual void OnClickButton(int buttonIndex) { }
}

/*
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
*/