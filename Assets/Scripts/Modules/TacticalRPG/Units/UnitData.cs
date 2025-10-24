using System.Collections.Generic;
using TacticalRPG.Skills;
using UnityEngine;

namespace TacticalRPG.Units
{
    /// <summary>
    /// Represents the base data for a playable unit in the game.
    /// Used as a ScriptableObject for easy configuration in the Unity editor.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("Base Stats")]
        [SerializeField] private int baseHP;
        [SerializeField] private int baseSP;
        [SerializeField] private int baseAttack;
        [SerializeField] private int baseDefense;
        [SerializeField] private int baseSpecialAttack;
        [SerializeField] private int baseSpecialDefense;
        [SerializeField] private int baseSpeed;

        [Header("Stat Gains Per Level")]
        [SerializeField, Range(0, 10)] private int gainHP;
        [SerializeField, Range(0, 10)] private int gainSP;
        [SerializeField, Range(0, 10)] private int gainAttack;
        [SerializeField, Range(0, 10)] private int gainDefense;
        [SerializeField, Range(0, 10)] private int gainSpecialAttack;
        [SerializeField, Range(0, 10)] private int gainSpecialDefense;
        [SerializeField, Range(0, 10)] private int gainSpeed;

        [Header("Critical")]
        [SerializeField, Range(0f, 1f)] private float criticalRate;
        [SerializeField, Range(1f, 3f)] private float criticalDamage;

        [Header("Movement")]
        [SerializeField] private int movementRange;
        [SerializeField] private int heightJump;
        [SerializeField] private int heightFall;

        [Header("Skills")]
        [SerializeField] private List<SkillData> _skills = new();

        /// <summary>Base health points.</summary>
        public int BaseHP => baseHP;
        /// <summary>Base SP points.</summary>
        public int BaseSP => baseSP;
        /// <summary>Base attack value.</summary>
        public int BaseAttack => baseAttack;
        /// <summary>Base defense value.</summary>
        public int BaseDefense => baseDefense;
        /// <summary>Base special attack value.</summary>
        public int BaseSpecialAttack => baseSpecialAttack;
        /// <summary>Base special defense value.</summary>
        public int BaseSpecialDefense => baseSpecialDefense;
        /// <summary>Base speed value.</summary>
        public int BaseSpeed => baseSpeed;

        /// <summary>Attack gain per level.</summary>
        public int GainAttack => gainAttack;
        /// <summary>Defense gain per level.</summary>
        public int GainDefense => gainDefense;
        /// <summary>Special attack gain per level.</summary>
        public int GainSpecialAttack => gainSpecialAttack;
        /// <summary>Special defense gain per level.</summary>
        public int GainSpecialDefense => gainSpecialDefense;
        /// <summary>Speed gain per level.</summary>
        public int GainSpeed => gainSpeed;

        /// <summary>Critical hit rate (0 to 1).</summary>
        public float CriticalRate => criticalRate;
        /// <summary>Critical damage multiplier.</summary>
        public float CriticalDamage => criticalDamage;

        /// <summary>Movement range.</summary>
        public int MovementRange => movementRange;
        /// <summary>Maximum jump height.</summary>
        public int HeightJump => heightJump;
        /// <summary>Maximum fall height.</summary>
        public int HeightFall => heightFall;
        /// <summary>
        ///  
        /// </summary>
        public List<SkillData> Skills => _skills;
    }
}