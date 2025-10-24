using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Playable Character")]
public class PlayableCharacterData : ScriptableObject
{
    [Header("General Information")]
    public CharacterGeneralData general;

    [Header("Personality Settings")]
    public CharacterPersonalityData personality;
}