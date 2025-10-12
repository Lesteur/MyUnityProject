using UnityEngine;
using Metroidvania.Core;
using Metroidvania.Core.States;
using Codice.Client.BaseCommands;

/// <summary>
/// Represents the jump state for a playable character.
/// Handles jump initiation, animation, and transitions to other states.
/// </summary>
public class CharacterStateJump : CharacterState
{
    private bool _hasJumped;

    /// <summary>
    /// Constructs the jump state for the character.
    /// </summary>
    /// <param name="stateMachine">Reference to the character's state machine.</param>
    public CharacterStateJump(CharacterStateMachine stateMachine) : base(stateMachine) { }

    /// <summary>
    /// Called when entering the jump state.
    /// Initiates the jump and plays the jump animation.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    public override void Enter(CharacterState previousState)
    {
        _hasJumped = false;
        Character.Animator.Play("Jump");

        Debug.Log("Jump State Entered");

        if (Character.IsJumpCutting)
            Character.Rb.linearVelocity = new Vector2(Character.Rb.linearVelocity.x, Character.JumpForce * Character.JumpCutMultiplier);
        else
            Character.Rb.linearVelocity = new Vector2(Character.Rb.linearVelocity.x, Character.JumpForce);

        _hasJumped = true;
    }

    /// <summary>
    /// Called every frame to update logic.
    /// Transitions to idle or move state when landing.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        ApplyVariableGravity();

        // If falling and grounded, transition to idle or move
        if (_hasJumped && Character.IsGrounded)
        {
            if (Mathf.Abs(Character.MoveInput.x) > 0.1f)
                StateMachine.EnterState(StateMachine.MoveState);
            else
                StateMachine.EnterState(StateMachine.IdleState);
        }
    }

    /// <summary>
    /// Called every physics frame to update movement.
    /// Allows for air control.
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // Air control: allow horizontal movement while jumping
        Character.Rb.linearVelocity = new Vector2(Character.MoveInput.x * Character.MoveSpeed, Character.Rb.linearVelocity.y);
    }

    /// <summary>
    /// Controls gravity based on jump state.
    /// </summary>
    private void ApplyVariableGravity()
    {
        if (Character.Rb.linearVelocity.y > 0.1f) // Ascending
        {
            Character.Rb.gravityScale = Character.JumpGravity;
        }
        else if (Character.Rb.linearVelocity.y < -0.1f) // Descending
        {
            Character.Rb.gravityScale = Character.FallGravity;
        }
        else // Neutral
        {
            Character.Rb.gravityScale = Character.NormalGravity;
        }
    }
}