using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] float dayLengthSeconds = 20;
    [SerializeField] float nightLengthSeconds = 20;
    float totalDNLength;
    float time = 0;

    [SerializeField] float transitionPercentage;
    float transitionSecondsToNight;
    float transitionSecondsToDay;

    private bool dayNight;
    public bool DayNight
    {
        get
        {
            return dayNight;
        }
        set
        {
            if (dayNight != value) { dayNight = value; OnDayNightChange(); };
        }
    }

    Volume dayNightVolume;

    private void Start()
    {
        totalDNLength = dayLengthSeconds + nightLengthSeconds;

        transitionSecondsToNight = ((transitionPercentage / 100) * dayLengthSeconds);
        transitionSecondsToDay = ((transitionPercentage / 100) * nightLengthSeconds);

        dayNightVolume = GetComponent<Volume>();
    }

    private void Update()
    {
        // if day
        if (time < dayLengthSeconds)
        {
            // if near end of day
            if (time > dayLengthSeconds - transitionSecondsToNight)
            {
                dayNightVolume.weight = (time - (dayLengthSeconds - transitionSecondsToNight)) / transitionSecondsToNight;
            }
        }

        // if night
        if (time > dayLengthSeconds)
        {
            DayNight = false;

            // if near end of night
            if (time < totalDNLength && time > totalDNLength - transitionSecondsToDay)
            {
                dayNightVolume.weight = 1 - (time - (totalDNLength - transitionSecondsToDay)) / transitionSecondsToDay;
            }
        }

        if (time < totalDNLength)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
            DayNight = true;
        }
    }

    void OnDayNightChange()
    {

    }
}
