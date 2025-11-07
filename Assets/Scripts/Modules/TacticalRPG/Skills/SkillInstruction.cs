using UnityEngine;

namespace TacticalRPG.Skills
{
    [System.Serializable]
    public class SkillInstruction// : ScriptableObject
    {
        public enum InstructionType
        {
            Damage,
            Heal,
            Buff,
            Debuff,
            ApplyStatusEffect,
            RemoveStatusEffect,
            ModifyStat,
            MoveTarget,
            SpawnObject,
            CustomAction
        }
        public InstructionType Type;// { get; private set; }
        public float Value; // { get; private set; }
        public float Duration; // { get; private set; }
        public string CustomActionName;// { get; private set; }

        /*
        public SkillInstruction(InstructionType type, float value = 0f, float duration = 0f, string customActionName = null)
        {
            Type = type;
            Value = value;
            Duration = duration;
            CustomActionName = customActionName;
        }
        */
    }
}