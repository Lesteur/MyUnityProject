using UnityEngine;
using System.Collections.Generic;
using TacticalRPG.Units;
using TacticalRPG.Paths;

namespace TacticalRPG.Core.States
{
    /// <summary>
    /// State responsible for handling the enemy turn in the tactical state machine.
    /// </summary>
    public class TacticalStateEnemyTurn : TacticalStateBase
    {
        private PathResult _selectedPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TacticalStateEnemyTurn"/> class.
        /// </summary>
        /// <param name="stateMachine">The state machine managing this state.</param>
        public TacticalStateEnemyTurn(TacticalStateMachine stateMachine) : base(stateMachine)
        {
            _selectedPath = default; // struct default (invalid state)
        }

        /// <inheritdoc/>
        public override void Enter(TacticalStateBase previousState)
        {
            Debug.Log("Entering Enemy Turn State");

            // Automatically select the first enemy unit that hasn't ended its turn
            foreach (Unit enemy in Controller.AllUnits)
            {
                if (enemy.Type == Unit.UnitType.Enemy && !enemy.EndTurn)
                {
                    Controller.SelectUnit(enemy);
                    break;
                }
            }

            // Find a path to the nearest player unit
            if (SelectedUnit != null)
            {
                List<PathResult> paths = Controller.Pathfinding.GetAllPathsFrom(SelectedUnit.GridPosition, SelectedUnit);
                Unit nearestPlayer = null;
                int shortestDistance = int.MaxValue;

                foreach (Unit unit in Controller.AlliedUnits)
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
                            _selectedPath = path;
                            distanceToPlayer = pathDistanceToPlayer;
                        }
                    }
                }

                if (_selectedPath.IsValid)
                {
                    Controller.MoveUnitPath(SelectedUnit, _selectedPath);
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
}