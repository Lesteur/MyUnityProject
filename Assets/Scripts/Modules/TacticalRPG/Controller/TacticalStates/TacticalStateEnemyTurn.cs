using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// State responsible for handling the enemy turn in the tactical state machine.
/// </summary>
public class TacticalStateEnemyTurn : TacticalStateBase
{
    private Unit _selectedUnit => TacticalController.Instance.SelectedUnit;
    private PathResult _selectedPath;

    /// <summary>
    /// Initializes a new instance of the enemy turn state.
    /// </summary>
    public TacticalStateEnemyTurn(TacticalStateMachine stateMachine) : base(stateMachine)
    {
        _selectedPath = default; // struct default (invalid state)
    }

    /// <inheritdoc/>
    public override void Enter(TacticalStateBase previousState)
    {
        Debug.Log("Entering Enemy Turn State");

        // Automatically select the first enemy unit that hasn't ended its turn
        foreach (Unit enemy in TacticalController.Instance.AllUnits)
        {
            if (enemy.Type == Unit.UnitType.Enemy && !enemy.EndTurn)
            {
                stateMachine.Controller.SelectUnit(enemy);
                break;
            }
        }

        // Find a path to the nearest player unit
        if (_selectedUnit != null)
        {
            List<PathResult> paths = TacticalController.Instance.Pathfinding.GetAllPathsFrom(_selectedUnit.GridPosition, _selectedUnit);
            Unit nearestPlayer = null;
            int shortestDistance = int.MaxValue;

            foreach (Unit unit in TacticalController.Instance.AlliedUnits)
            {
                int distance = Mathf.Abs(unit.GridPosition.x - _selectedUnit.GridPosition.x) +
                                Mathf.Abs(unit.GridPosition.y - _selectedUnit.GridPosition.y);
                
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
                        _selectedPath = path;
                        distanceToPlayer = pathDistanceToPlayer;
                    }
                }
            }

            if (_selectedPath.IsValid)
            {
                TacticalController.Instance.MoveUnitPath(_selectedUnit, _selectedPath);
            }
            else
            {
                Debug.LogWarning("No valid path found to the nearest player unit.");
                _selectedUnit.EndTurn = true; // End turn if no path is found
            }
        }
        else
        {
            Debug.LogWarning("No enemy unit available to take a turn.");
        }
    }
}