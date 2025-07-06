using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;

/// <summary>
/// Represents a visual novel actor with a name, default sprite, and mood-based face expressions.
/// Stored as a ScriptableObject for modular data-driven design.
/// </summary>
[CreateAssetMenu(fileName = "NewActor", menuName = "VisualNovel/Actor")]
public class Actor : ScriptableObject
{
    /// <summary>
    /// Unique identifier for the actor (used in scripts and lookups).
    /// </summary>
    public string id;

    /// <summary>
    /// Localized display name shown in the UI.
    /// </summary>
    public LocalizedString displayName;

    /// <summary>
    /// The default body sprite for this actor.
    /// </summary>
    public Sprite defaultBody;

    /// <summary>
    /// List of face expressions (moods) mapped by ID.
    /// </summary>
    public List<MoodSprite> moods;

    /// <summary>
    /// Struct representing a face sprite tied to a mood.
    /// </summary>
    [System.Serializable]
    public struct MoodSprite
    {
        public string moodId;  // Mood identifier string (e.g., "happy", "angry")
        public Sprite face;    // Sprite representing the mood
    }

    /// <summary>
    /// Retrieves the face sprite for a given mood.
    /// </summary>
    /// <param name="mood">Mood identifier.</param>
    /// <returns>Corresponding sprite, or null if not found.</returns>
    public Sprite GetMoodSprite(string mood)
    {
        foreach (var m in moods)
        {
            if (m.moodId == mood)
                return m.face;
        }

        return null;
    }
}