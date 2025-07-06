using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class CharacterGeneralData
{
    public string id;
    
    public LocalizedString name;
    public LocalizedString fullName;
    public LocalizedString race;
    public LocalizedString charClass;
    public LocalizedString description;

    public int age;
    public Color color;
    public Gender gender;
    public Affiliation affiliation;
}