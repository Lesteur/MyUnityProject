using UnityEngine;
using UnityEngine.Localization;

namespace TacticalRPG.Skills
{
    /// <summary>
    /// Defines the data for a skill, including cost, range, effects, and visuals.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkill", menuName = "TacticalRPG/Skill")]
    public class SkillData : ScriptableObject
    {
        [Header("Meta")]
        [SerializeField] private LocalizedString _skillName;
        [SerializeField] private LocalizedString _description;
        [SerializeField] private Sprite _icon;

        [Header("Type & Cost")]
        [SerializeField] private SkillType _skillType;
        [SerializeField] private TargetType _targetType;
        [SerializeField] private int _skillPointsCost;
        [SerializeField] private int _power;
        [SerializeField, Range(0, 5)] private int _cooldownTurns;
        [SerializeField, Range(0f, 1f)] private float _accuracy = 1f;
        [SerializeField, Range(0f, 1f)] private float _criticalRate = 0f;

        [Header("Range")]
        [SerializeField] private SkillArea _areaOfEffect;
        [SerializeField] private SkillArea _areaOfEffectSecondary;

        [Header("Behavior Flags")]
        [SerializeField] private bool _canTargetSelf = false;
        [SerializeField] private bool _requiresLineOfSight = true;

        [Header("Visuals & FX")]
        [SerializeField] private GameObject _effectPrefab;
        [SerializeField] private AudioClip _soundEffect;

        #region Properties

        /// <summary> The localized display name of the skill. </summary>
        public LocalizedString SkillName => _skillName;

        /// <summary> The localized description of the skill. </summary>
        public LocalizedString Description => _description;

        /// <summary> The skill's icon used in UI. </summary>
        public Sprite Icon => _icon;

        /// <summary> The category/type of the skill (e.g., magic, physical). </summary>
        public SkillType SkillType => _skillType;

        /// <summary> Defines what type of targets this skill can affect. </summary>
        public TargetType TargetType => _targetType;

        /// <summary> The SP cost required to use the skill. </summary>
        public int SkillPointsCost => _skillPointsCost;

        /// <summary> The base power value used for damage or healing. </summary>
        public int Power => _power;

        /// <summary> Number of turns before the skill can be reused. </summary>
        public int CooldownTurns => _cooldownTurns;

        /// <summary> Hit chance modifier, from 0 (never hits) to 1 (always hits). </summary>
        public float Accuracy => _accuracy;

        /// <summary> Chance of critical hit, from 0 to 1. </summary>
        public float CriticalRate => _criticalRate;

        /// <summary> The area of effect pattern for this skill. </summary>
        public SkillArea AreaOfEffect => _areaOfEffect;

        /// <summary> The secondary area of effect pattern for this skill (optional, e.g., caster's area). </summary>
        public SkillArea AreaOfEffectSecondary => _areaOfEffectSecondary;

        /// <summary> Whether the caster can target themselves. </summary>
        public bool CanTargetSelf => _canTargetSelf;

        /// <summary> Whether line of sight is required to use this skill. </summary>
        public bool RequiresLineOfSight => _requiresLineOfSight;

        /// <summary> The prefab spawned when the skill is used. </summary>
        public GameObject EffectPrefab => _effectPrefab;

        /// <summary> The audio clip played when the skill is activated. </summary>
        public AudioClip SoundEffect => _soundEffect;

        #endregion
    }
}