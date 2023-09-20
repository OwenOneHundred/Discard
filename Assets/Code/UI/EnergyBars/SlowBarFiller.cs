using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowBarFiller : MonoBehaviour
{
    [SerializeField] float increaseSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float startValue;
    public Slider slider;
    [SerializeField] bool slowDownExpolly = true;

    public event Action<SlowBarFiller> onFill;

    private void Start()
    {
        slider.value = startValue;
    }

    private void Update()
    {
        float maxValue = slider.maxValue;
        if (slider.value < maxValue)
        {
            if (slowDownExpolly)
            {
                slider.value += Time.deltaTime * Mathf.Clamp(increaseSpeed - ((slider.value / maxValue) * increaseSpeed), minSpeed, 10000);
            }
            else
            {
                slider.value += Time.deltaTime * increaseSpeed;
            }
        }
        else
        {
            onFill?.Invoke(this);
        }
    }
}
