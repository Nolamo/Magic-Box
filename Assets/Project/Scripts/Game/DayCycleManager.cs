using NolanCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class TimeUpdateEventArgs : EventArgs
{
    public int totalMinutes;

    public int hour;
    public int minute;
}

public class DayCycleManager : SingletonMonobehaviour<DayCycleManager>
{
    // how many minutes are in a day?
    private const int TOTAL_MINUTES = 60 * 24; // 1440 :P

    [field: SerializeField, ReadOnly] public int elapsedDays { get; set; } = 0;

    // the length of one day in seconds
    // default is 72 minutes
    [field: SerializeField] public float dayDuration { get; set; } = 4320;
    // the speed at which one second passes
    [field: SerializeField] public float timespeedMultiplier { get; set; } = 1;

    // 0 = midnight, daylength / 2 = 12:00
    [field: SerializeField] public float currentTime { get; set; }

    public float dayPercent => currentTime / dayDuration;

    public float minuteDuration => dayDuration / 24 / 60;

    private float _elapsedMinute;

    public EventHandler<TimeUpdateEventArgs> OnMinuteChanged;
    public EventHandler<TimeUpdateEventArgs> OnDayChanged;

    private void Update()
    {
        // update the time and invoke a delegate when the day changes
        currentTime += Time.deltaTime * timespeedMultiplier;

        if (currentTime >= dayDuration)
        {
            currentTime -= dayDuration;
            elapsedDays++;
            OnDayChanged?.Invoke
                (this, 
                new TimeUpdateEventArgs
                {
                    hour = GetHour(),
                    minute = GetMinute(),
                    totalMinutes = GetTotalMinutes()
                }
                );
        }

        // count the minutes and invoke a delegate when the minute changes
        _elapsedMinute += Time.deltaTime;

        if(_elapsedMinute >= minuteDuration)
        {
            _elapsedMinute -= minuteDuration;
            OnMinuteChanged?.Invoke
                (this,
                new TimeUpdateEventArgs
                {
                    hour = GetHour(),
                    minute = GetMinute(),
                    totalMinutes = GetTotalMinutes()
                }
                );
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1)) Debug.Log($"It's {GetStandardTime()} or {GetMilitaryTime()}");
    }
#endif

    // get a string that represents the time by 24 hour clock
    public string GetMilitaryTime()
    {
        return $"{GetHour():D2}:{GetMinute():D2}";
    }

    // get a string that represents the time by 12 hour clock
    public string GetStandardTime(bool includePeriod = true)
    {
        int hour = (GetHour() % 12 == 0) ? 12 : GetHour() % 12;

        string period = "";

        if (includePeriod) period = (GetHour() >= 12) ? "PM" : "AM";

        return $"{hour:D2}:{GetMinute():D2} {period}";
    }

    // get the current hour
    public int GetHour()
    {
        int hour = GetTotalMinutes() / 60;
        return hour;
    }

    // get the current minute of the current hour
    public int GetMinute()
    {
        int minute = GetTotalMinutes() % 60;
        return minute;
    }

    // get the total minutes passed in this day
    public int GetTotalMinutes()
    {
        int minutesPassed = (int)(TOTAL_MINUTES * dayPercent);
        return minutesPassed;
    }
}
