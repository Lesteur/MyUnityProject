using UnityEngine;
using System.Collections.Generic;

using TacticalRPG.Units;

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
        /// The primary target of the skill.
        /// </summary>
        public Unit Target { get; private set; }
        /// <summary>
        /// The position where the skill is being used.
        /// </summary>
        public Vector2Int Position { get; private set; }
        /// <summary>
        /// Additional parameters or modifiers for the skill use.
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }
        /// <summary>
        /// Initializes a new instance of the SkillContext class.
        /// </summary>
        /// <param name="user">The unit using the skill.</param>
        /// <param name="target">The primary target of the skill.</param>
        /// <param name="position">The position where the skill is being used.</param>
        /// <param name="parameters">Additional parameters or modifiers for the skill use.</param>
        public SkillContext(Unit user, Unit target, Vector2Int position, Dictionary<string, object> parameters = null)
        {
            User = user;
            Target = target;
            Position = position;
            Parameters = parameters ?? new Dictionary<string, object>();
        }
    }
}