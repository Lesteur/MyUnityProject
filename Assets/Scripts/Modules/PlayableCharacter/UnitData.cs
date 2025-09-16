using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    [SerializeField] private int baseHP;
    [SerializeField] private int baseSP;

    [SerializeField] private int baseAttack;
    [SerializeField] private int baseDefense;
    [SerializeField] private int baseSpecialAttack;
    [SerializeField] private int baseSpecialDefense;
    [SerializeField] private int baseSpeed;

    [SerializeField][Range(0, 10)] private int gainAttack;
    [SerializeField][Range(0, 10)] private int gainDefense;
    [SerializeField][Range(0, 10)] private int gainSpecialAttack;
    [SerializeField][Range(0, 10)] private int gainSpecialDefense;
    [SerializeField][Range(0, 10)] private int gainSpeed;

    [SerializeField][Range(0f, 1f)] private float criticalRate;
    [SerializeField][Range(1f, 3f)] private float criticalDamage;

    [SerializeField] private int movementRange;
    [SerializeField] private int heightJump;

    int HP => baseHP;
    int SP => baseSP;
    int Attack => baseAttack;
    int Defense => baseDefense;
    int SpecialAttack => baseSpecialAttack;
    int SpecialDefense => baseSpecialDefense;
    int Speed => baseSpeed;
    int GainAttack => gainAttack;
    int GainDefense => gainDefense;
    int GainSpecialAttack => gainSpecialAttack;
    int GainSpecialDefense => gainSpecialDefense;
    int GainSpeed => gainSpeed;
    float CriticalRate => criticalRate;
    float CriticalDamage => criticalDamage;
    int MovementRange => movementRange;
}