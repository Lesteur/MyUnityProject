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
        [SerializeField] private int _baseHP;
        [SerializeField] private int _baseSP;
        [SerializeField] private int _baseAttack;
        [SerializeField] private int _baseDefense;
        [SerializeField] private int _baseSpecialAttack;
        [SerializeField] private int _baseSpecialDefense;
        [SerializeField] private int _baseSpeed;

        [Header("Stat Gains Per Level")]
        [SerializeField, Range(0, 10)] private int _gainHP;
        [SerializeField, Range(0, 10)] private int _gainSP;
        [SerializeField, Range(0, 10)] private int _gainAttack;
        [SerializeField, Range(0, 10)] private int _gainDefense;
        [SerializeField, Range(0, 10)] private int _gainSpecialAttack;
        [SerializeField, Range(0, 10)] private int _gainSpecialDefense;
        [SerializeField, Range(0, 10)] private int _gainSpeed;

        [Header("Critical")]
        [SerializeField, Range(0f, 1f)] private float _criticalRate;
        [SerializeField, Range(1f, 3f)] private float _criticalDamage;

        [Header("Movement")]
        [SerializeField] private int _movementRange;
        [SerializeField] private int _jumpHeight;
        [SerializeField] private int _fallHeight;

        [Header("Skills")]
        [SerializeField] private List<SkillData> _skills = new();

        /// <summary>Base health points.</summary>
        public int BaseHP => _baseHP;
        /// <summary>Base SP points.</summary>
        public int BaseSP => _baseSP;
        /// <summary>Base attack value.</summary>
        public int BaseAttack => _baseAttack;
        /// <summary>Base defense value.</summary>
        public int BaseDefense => _baseDefense;
        /// <summary>Base special attack value.</summary>
        public int BaseSpecialAttack => _baseSpecialAttack;
        /// <summary>Base special defense value.</summary>
        public int BaseSpecialDefense => _baseSpecialDefense;
        /// <summary>Base speed value.</summary>
        public int BaseSpeed => _baseSpeed;

        /// <summary>Health points gain per level.</summary>
        public int GainHP => _gainHP;
        /// <summary>SP gain per level.</summary>
        public int GainSP => _gainSP;
        /// <summary>Attack gain per level.</summary>
        public int GainAttack => _gainAttack;
        /// <summary>Defense gain per level.</summary>
        public int GainDefense => _gainDefense;
        /// <summary>Special attack gain per level.</summary>
        public int GainSpecialAttack => _gainSpecialAttack;
        /// <summary>Special defense gain per level.</summary>
        public int GainSpecialDefense => _gainSpecialDefense;
        /// <summary>Speed gain per level.</summary>
        public int GainSpeed => _gainSpeed;

        /// <summary>Critical hit rate (0 to 1).</summary>
        public float CriticalRate => _criticalRate;
        /// <summary>Critical damage multiplier.</summary>
        public float CriticalDamage => _criticalDamage;

        /// <summary>Movement range.</summary>
        public int MovementRange => _movementRange;
        /// <summary>Maximum jump height.</summary>
        public int JumpHeight => _jumpHeight;
        /// <summary>Maximum fall height.</summary>
        public int FallHeight => _fallHeight;

        /// <summary>List of skills available to the unit.</summary>
        public List<SkillData> Skills => _skills;
    }
}