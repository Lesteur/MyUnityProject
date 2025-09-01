using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Skill", menuName = "TacticalRPG/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Meta")]
    public LocalizedString skillName;
    [TextArea] public LocalizedString description;
    public Sprite icon;

    [Header("Type & Cost")]
    public SkillType skillType;
    public TargetType targetType;
    public int spCost;
    public int power;
    public int accuracy = 100;
    public int criticalRate = 0;

    [Header("Range")]
    public AttackPattern pattern;

    [Header("Behavior Flags")]
    public bool canTargetSelf = false;
    public bool requiresLineOfSight = true;

    [Header("Visuals & FX")]
    public GameObject effectPrefab;
    public AudioClip soundEffect;
}