using UnityEngine;
using Metroidvania.Core;
using Metroidvania.Core.States;

/// <summary>
/// Represents the idle state for a playable character.
/// Handles entry logic and transitions to movement or jump state when input is detected.
/// </summary>
public class CharacterStateIdle : CharacterState
{
    /// <summary>
    /// Constructs the idle state for the character.
    /// </summary>
    /// <param name="stateMachine">Reference to the character's state machine.</param>
    public CharacterStateIdle(CharacterStateMachine stateMachine) : base(stateMachine) { }

    /// <summary>
    /// Called when entering the idle state.
    /// Stops horizontal movement and plays the idle animation.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    public override void Enter(CharacterState previousState)
    {
        Character.Rb.linearVelocity = new Vector2(0f, Character.Rb.linearVelocity.y);
        Character.Animator.Play("Idle");

        Debug.Log("Idle State Entered");
    }

    /// <summary>
    /// Called every frame to update logic.
    /// Transitions to move state if horizontal input is detected.
    /// Transitions to jump state if jump input is detected and grounded.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Mathf.Abs(Character.MoveInput.x) > 0.1f)
        {
            StateMachine.EnterState(StateMachine.MoveState);
        }
        else if (Character.JumpRequested && Character.IsGrounded)
        {
            StateMachine.EnterState(StateMachine.JumpState);
        }
    }
}