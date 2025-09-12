using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Skill", menuName = "TacticalRPG/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Meta")]
    public LocalizedString skillName;
    public LocalizedString description;
    public Sprite icon;

    [Header("Type & Cost")]
    public SkillType skillType;
    public TargetType targetType;
    public int spCost;
    public int power;
    [Range(0, 5)] public int cooldownTurns;
    [Range(0f, 1f)] public float accuracy = 1f;
    [Range(0f, 1f)] public float criticalRate = 0f;

    [Header("Range")]
    public SkillArea areaOfEffect;

    [Header("Behavior Flags")]
    public bool canTargetSelf = false;
    public bool requiresLineOfSight = true;

    [Header("Visuals & FX")]
    public GameObject effectPrefab;
    public AudioClip soundEffect;
}