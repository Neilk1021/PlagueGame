using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;

public class HealthUi : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void UpdateHealthBar(float currentHealth)
    {
        slider.value = currentHealth;
    }

    public void UpdateHealthBar()
    {
        slider.value = 1f;
    }
}
