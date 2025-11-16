using UnityEngine.UI;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// A simple health bar component that can be attached to a UI element.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        /// <summary>
        /// The slider component representing the health bar.
        /// </summary>
        private Slider _slider;

        /// <summary>
        /// Initializes the health bar by getting the Slider component.
        /// </summary>
        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        /// <summary>
        /// Sets the health value of the health bar.
        /// </summary>
        public void SetHealth(float health, float maxHealth)
        {
            if (_slider != null)
            {
                _slider.value = health / maxHealth;
            }
        }
    }
}