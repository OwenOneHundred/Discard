using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    Slider slider;
    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    /// <summary>
    /// Changes slider value. Enter a value from 0 to 1, where 1 is full health.
    /// </summary>
    public void UpdateSlider(float value)
    {
        slider.value = value;
    }
}
