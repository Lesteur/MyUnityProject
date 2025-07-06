using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CharacterPersonalityData
{
    public List<PersonalityTrait> personalityTraits = new List<PersonalityTrait>(6);

    // Goûts alimentaires : du préféré au détesté
    public List<FoodType> foodTastes;
    public List<InterestType> interests;
    public List<GiftType> giftTastes;
    public List<BookType> bookTastes;

    public Alignment alignment;

    #if UNITY_EDITOR
    public void Validate()
    {
        if (personalityTraits.Count != 3)
        {
            Debug.LogWarning("Character must have exactly 6 personality traits.");
        }

        var hashSet = new HashSet<PersonalityTrait>();
        foreach (var trait in personalityTraits)
        {
            if (!hashSet.Add(trait))
            {
                Debug.LogWarning("Duplicate personality trait found.");
            }
        }
    }
    #endif
}