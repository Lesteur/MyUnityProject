using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// State responsible for handling the enemy turn in the tactical state machine.
/// </summary>
public class TacticalStateEnemyTurn : TacticalStateBase
{
    private Unit SelectedUnit => stateMachine.Controller.SelectedUnit;
    private PathResult selectedPath;

    /// <summary>
    /// Initializes a new instance of the enemy turn state.
    /// </summary>
    public TacticalStateEnemyTurn(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        selectedPath = default; // struct default (invalid state)
    }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Enemy Turn State");

        // Automatically select the first enemy unit that hasn't ended its turn
        foreach (Unit enemy in stateMachine.Controller.Units)
        {
            if (enemy.Type == Unit.UnitType.Enemy && !enemy.EndTurn)
            {
                stateMachine.Controller.SelectUnit(enemy);
                break;
            }
        }

        // Find a path to the nearest player unit
        if (SelectedUnit != null)
        {
            List<PathResult> paths = stateMachine.Controller.Pathfinding.GetAllPathsFrom(SelectedUnit.GridPosition, SelectedUnit);
            Unit nearestPlayer = null;
            int shortestDistance = int.MaxValue;

            foreach (Unit unit in stateMachine.Controller.AlliedUnits)
            {
                int distance = Mathf.Abs(unit.GridPosition.x - SelectedUnit.GridPosition.x) +
                                Mathf.Abs(unit.GridPosition.y - SelectedUnit.GridPosition.y);
                
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPlayer = unit;
                }
            }

            if (nearestPlayer != null)
            {
                int distanceToPlayer = int.MaxValue;

                foreach (var path in paths)
                {
                    var pathEnd = path.Destination;
                    var pathDistanceToPlayer = Mathf.Abs(nearestPlayer.GridPosition.x - pathEnd.GridPosition.x) +
                                               Mathf.Abs(nearestPlayer.GridPosition.y - pathEnd.GridPosition.y);

                    if (pathDistanceToPlayer < distanceToPlayer)
                    {
                        selectedPath = path;
                        distanceToPlayer = pathDistanceToPlayer;
                    }
                }
            }

            if (selectedPath.IsValid)
            {
                TacticalController.Instance.MoveUnitPath(SelectedUnit, selectedPath);
            }
            else
            {
                Debug.LogWarning("No valid path found to the nearest player unit.");
                SelectedUnit.EndTurn = true; // End turn if no path is found
            }
        }
        else
        {
            Debug.LogWarning("No enemy unit available to take a turn.");
        }
    }
}