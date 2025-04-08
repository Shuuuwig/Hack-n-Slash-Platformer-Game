using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMaxHealth(float maxHealthValue)
    {
        slider.maxValue = maxHealthValue;
        slider.value = maxHealthValue;
    }

    public void SetHealthBar(float currentHealthValue)
    {
        slider.value = currentHealthValue;
    }
}
