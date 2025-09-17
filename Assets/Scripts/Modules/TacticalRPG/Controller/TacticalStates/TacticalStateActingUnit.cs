using UnityEngine;

/// <summary>
/// State responsible for displaying and handling interactions with the tactical main menu.
/// </summary>
public class TacticalStateActingUnit : TacticalStateBase
{
    /// <summary>
    /// Initializes a new instance of the main menu state.
    /// </summary>
    public TacticalStateActingUnit(TacticalStateMachine stateMachine) : base(stateMachine) { }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Acting Unit State");
    }
}