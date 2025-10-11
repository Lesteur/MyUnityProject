using UnityEngine;
using Metroidvania.Core;
using Metroidvania.Core.States;

/// <summary>
/// Represents the movement state for a playable character.
/// Handles movement logic and transitions to idle state when input ceases.
/// </summary>
public class CharacterStateMove : CharacterState
{
    /// <summary>
    /// Constructs the move state for the character.
    /// </summary>
    /// <param name="stateMachine">Reference to the character's state machine.</param>
    public CharacterStateMove(CharacterStateMachine stateMachine) : base(stateMachine) { }

    /// <summary>
    /// Called when entering the move state.
    /// Plays the run animation.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    public override void Enter(CharacterState previousState)
    {
        Character.Animator.Play("Run");

        Debug.Log("Move State Entered");
    }

    /// <summary>
    /// Called every frame to update logic.
    /// Transitions to idle state if horizontal input is not detected.
    /// Transitions to jump state if jump input is detected and grounded.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Mathf.Abs(Character.MoveInput.x) < 0.1f)
        {
            StateMachine.EnterState(StateMachine.IdleState);
        }
        else if (Character.JumpRequested && Character.IsGrounded)
        {
            StateMachine.EnterState(StateMachine.JumpState);
        }
    }

    /// <summary>
    /// Called every physics frame to update movement.
    /// Applies horizontal movement based on input.
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Character.Rb.linearVelocity = new Vector2(Character.MoveInput.x * Character.MoveSpeed, Character.Rb.linearVelocity.y);
    }
}