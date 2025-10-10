using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;

namespace VisualNovel
{
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
        [SerializeField] private string _id;
        /// <summary>
        /// Localized display name shown in the UI.
        /// </summary>
        [SerializeField] private LocalizedString _displayName;
        /// <summary>
        /// The default body sprite for this actor.
        /// </summary>
        [SerializeField] private Sprite _defaultBody;
        /// <summary>
        /// List of face expressions (moods) mapped by ID.
        /// </summary>
        [SerializeField] private List<MoodSprite> _moods = new();

        /// <summary>
        /// Gets the unique identifier for the actor.
        /// </summary>
        public string Id => _id;
        /// <summary>
        /// Gets the localized display name for the actor.
        /// </summary>
        public LocalizedString DisplayName => _displayName;
        /// <summary>
        /// Gets the default body sprite for the actor.
        /// </summary>
        public Sprite DefaultBody => _defaultBody;
        /// <summary>
        /// Gets the list of mood sprites for the actor.
        /// </summary>
        public List<MoodSprite> Moods => _moods;

        /// <summary>
        /// Struct representing a face sprite tied to a mood.
        /// </summary>
        [System.Serializable]
        public struct MoodSprite
        {
            /// <summary>
            /// Mood identifier string (e.g., "happy", "angry").
            /// </summary>
            public string moodId;
            /// <summary>
            /// Sprite representing the mood.
            /// </summary>
            public Sprite face;
        }

        /// <summary>
        /// Retrieves the face sprite for a given mood.
        /// </summary>
        /// <param name="mood">Mood identifier.</param>
        /// <returns>Corresponding sprite, or null if not found.</returns>
        public Sprite GetMoodSprite(string mood)
        {
            foreach (var m in _moods)
            {
                if (m.moodId == mood)
                    return m.face;
            }

            return null;
        }
    }
}