using UnityEngine;
using UnityEngine.UI;

namespace VisualNovel
{
    /// <summary>
    /// A UI container representing a slot where an actor can appear with a body and facial expression.
    /// </summary>
    public class ActorSlot : MonoBehaviour
    {
        [SerializeField] private Image _bodyImage;
        [SerializeField] private Image _faceImage;
        private Actor _currentActor;

        /// <summary>
        /// Assigns an actor to this slot and shows default visuals.
        /// </summary>
        /// <param name="actor">Actor to assign.</param>
        public void SetActor(Actor actor)
        {
            if (_bodyImage == null || _faceImage == null)
            {
                Debug.LogError("ActorSlot: UI Image components are missing.");
                return;
            }
            if (actor == null)
            {
                Debug.LogError("ActorSlot: Actor is null.");
                return;
            }

            _currentActor = actor;

            _bodyImage.sprite = actor.DefaultBody;
            _faceImage.sprite = actor.GetMoodSprite("default");
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Changes the actor's facial expression (mood).
        /// </summary>
        /// <param name="moodId">Mood identifier to switch to.</param>
        public void SetMood(string moodId)
        {
            if (_currentActor != null)
            {
                var moodSprite = _currentActor.GetMoodSprite(moodId);
                if (moodSprite != null)
                    _faceImage.sprite = moodSprite;
            }
        }

        /// <summary>
        /// Hides the actor's visuals from the scene.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Retrieves the currently assigned actor.
        /// </summary>
        /// <returns>The current actor or null.</returns>
        public Actor GetActor()
        {
            return _currentActor;
        }
    }
}