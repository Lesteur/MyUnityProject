using System.Collections.Generic;
using TacticalRPG.Core;
using TacticalRPG.Units;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TacticalRPG.Skills
{
    /// <summary>
    /// Represents the context in which a skill is used, including the user, target, and environment.
    /// </summary>
    public class SkillContext
    {
        /// <summary>
        /// The unit using the skill.
        /// </summary>
        public Unit User { get; private set; }
        /// <summary>
        /// The skill being used.
        /// </summary>
        public SkillData Skill { get; private set; }
        /// <summary>
        /// The tiles affected by the skill.
        /// </summary>
        public List<Tile> AffectedTiles { get; private set; }
        /// <summary>
        /// The additional parameters or modifiers for the skill use.
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// The list of target units affected by the skill.
        /// </summary>
        public List<Unit> Targets { get; private set; } = new List<Unit>();
        /// <summary>
        /// Initializes a new instance of the SkillContext class.
        /// </summary>
        /// <param name="user">The unit using the skill.</param>
        /// <param name="target">The primary target of the skill.</param>
        /// <param name="position">The position where the skill is being used.</param>
        /// <param name="parameters">Additional parameters or modifiers for the skill use.</param>
        public SkillContext(Unit user, SkillData skill, List<Tile> affectedTiles, Dictionary<string, object> parameters = null)
        {
            User = user;
            Skill = skill;
            AffectedTiles = affectedTiles;
            Parameters = parameters ?? new Dictionary<string, object>();

            foreach (var tile in AffectedTiles)
            {
                if (tile.OccupyingUnit != null && tile.OccupyingUnit != User)
                {
                    Targets.Add(tile.OccupyingUnit);
                }
            }
        }

        public void PhysicalAttack()
        {
            int power = Skill.Power + User.Attack;

            foreach (var target in Targets)
            {
                int damage = power - target.Defense;
                damage = Mathf.Max(damage, 1); // Ensure damage is not negative
                target.ApplyDamage(damage);
            }
        }
    }
}