using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// Defines the data for a skill, including cost, range, effects, and visuals.
/// </summary>
[CreateAssetMenu(fileName = "NewSkill", menuName = "TacticalRPG/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Meta")]
    [SerializeField] private LocalizedString skillName;
    [SerializeField] private LocalizedString description;
    [SerializeField] private Sprite icon;

    [Header("Type & Cost")]
    [SerializeField] private SkillType skillType;
    [SerializeField] private TargetType targetType;
    [SerializeField] private int spCost;
    [SerializeField] private int power;
    [SerializeField, Range(0, 5)] private int cooldownTurns;
    [SerializeField, Range(0f, 1f)] private float accuracy = 1f;
    [SerializeField, Range(0f, 1f)] private float criticalRate = 0f;

    [Header("Range")]
    [SerializeField] private SkillArea areaOfEffect;

    [Header("Behavior Flags")]
    [SerializeField] private bool canTargetSelf = false;
    [SerializeField] private bool requiresLineOfSight = true;

    [Header("Visuals & FX")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private AudioClip soundEffect;

    #region Properties

    /// <summary> The localized display name of the skill. </summary>
    public LocalizedString SkillName => skillName;

    /// <summary> The localized description of the skill. </summary>
    public LocalizedString Description => description;

    /// <summary> The skill's icon used in UI. </summary>
    public Sprite Icon => icon;

    /// <summary> The category/type of the skill (e.g., magic, physical). </summary>
    public SkillType SkillType => skillType;

    /// <summary> Defines what type of targets this skill can affect. </summary>
    public TargetType TargetType => targetType;

    /// <summary> The SP cost required to use the skill. </summary>
    public int SpCost => spCost;

    /// <summary> The base power value used for damage or healing. </summary>
    public int Power => power;

    /// <summary> Number of turns before the skill can be reused. </summary>
    public int CooldownTurns => cooldownTurns;

    /// <summary> Hit chance modifier, from 0 (never hits) to 1 (always hits). </summary>
    public float Accuracy => accuracy;

    /// <summary> Chance of critical hit, from 0 to 1. </summary>
    public float CriticalRate => criticalRate;

    /// <summary> The area of effect pattern for this skill. </summary>
    public SkillArea AreaOfEffect => areaOfEffect;

    /// <summary> Whether the caster can target themselves. </summary>
    public bool CanTargetSelf => canTargetSelf;

    /// <summary> Whether line of sight is required to use this skill. </summary>
    public bool RequiresLineOfSight => requiresLineOfSight;

    /// <summary> The prefab spawned when the skill is used. </summary>
    public GameObject EffectPrefab => effectPrefab;

    /// <summary> The audio clip played when the skill is activated. </summary>
    public AudioClip SoundEffect => soundEffect;

    #endregion
}