using UnityEngine.UI;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetHealth(float health, float maxHealth)
    {
        if (_slider != null)
        {
            _slider.value = health / maxHealth;
        }
    }
}