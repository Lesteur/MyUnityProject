using UnityEngine;
using System.Collections.Generic;

namespace TacticalRPG.Skills
{
    /// <summary>
    /// Defines the area of effect pattern for a skill, including range and shape.
    /// </summary>
    [System.Serializable]
    public class SkillArea
    {
        /// <summary>
        /// The geometric pattern used to calculate affected tiles.
        /// </summary>
        private enum AreaType
        {
            Circle,
            Square,
            Cross
        }

        [SerializeField, Min(0)] private int _minRange = 0;
        [SerializeField, Min(1)] private int _maxRange = 1;
        [SerializeField] private AreaType _areaType = AreaType.Circle;

        /// <summary> The shape of the skill's area of effect. </summary>
        //public AreaType Shape => areaType;

        /// <summary>
        /// Returns a list of grid positions affected by this skill area from a given origin tile.
        /// </summary>
        public List<Vector2Int> GetAllRangedPositions()
        {
            List<Vector2Int> affectedPositions = new List<Vector2Int>();
            
            for (int x = -_maxRange; x <= _maxRange; x++)
            {
                for (int y = -_maxRange; y <= _maxRange; y++)
                {
                    int distance = Mathf.Abs(x) + Mathf.Abs(y);

                    if (IsTileInArea(x, y, distance))
                    {
                        affectedPositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            return affectedPositions;
        }

        /// <summary>
        /// Checks whether a given offset is included in the current area type.
        /// </summary>
        private bool IsTileInArea(int x, int y, int distance)
        {
            switch (_areaType)
            {
                case AreaType.Circle:
                    return distance > _minRange && distance <= _maxRange;
                case AreaType.Square:
                    return Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) > _minRange && 
                        Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) <= _maxRange;
                case AreaType.Cross:
                    return (x == 0 || y == 0) && distance > _minRange && distance <= _maxRange;
                default:
                    return false;
            }
        }
    }
}