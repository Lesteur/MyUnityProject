using UnityEngine;

[System.Serializable]
public class CharacterFightData
{
    public int baseHP;
    public int baseSP;

    public int baseAttack;
    public int baseDefense;
    public int baseSpecialAttack;
    public int baseSpecialDefense;
    public int baseSpeed;

    [Range(0, 5)] public int gainAttack;
    [Range(0, 5)] public int gainDefense;
    [Range(0, 5)] public int gainSpecialAttack;
    [Range(0, 5)] public int gainSpecialDefense;
    [Range(0, 5)] public int gainSpeed;

    [Range(0f, 1f)] public float criticalRate;
    [Range(1f, 3f)] public float criticalDamage;

    public int movementRange;
    public int heightJump;
}