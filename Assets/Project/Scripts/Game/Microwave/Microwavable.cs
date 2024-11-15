using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Temperature))]
public class MicrowavableProp : MonoBehaviour, IMicrowavable
{
    public float cookProgress { get; private set; }

    [SerializeField] private List<MicrowaveEvent> MicrowaveEvents;

    [SerializeField] private float _microwaveTemperatureGain;

    private Temperature _temperature;

    void Awake()
    {
        if (MicrowaveEvents == null || MicrowaveEvents.Count <= 0) 
            return;

        MicrowaveEvents.Sort();
        _temperature = GetComponent<Temperature>();
    }

    public void Microwave(float amount)
    {
        cookProgress += amount;

        if (_temperature != null) _temperature._temperatureProperties.temperature += _microwaveTemperatureGain * Time.deltaTime;

        if (MicrowaveEvents == null || MicrowaveEvents.Count <= 0) 
            return;

        // check the lowest minimum progress microwave event to see if the minimum progress has been reached
        // when it gets reached, remove it, and loop.
        while (MicrowaveEvents[0].TryInvoke(cookProgress))
        {
            MicrowaveEvents.RemoveAt(0);
        }
    }

    // AddMicrowaveEvent re-sorts the events after adding. This is to prevent events not being ordered correctly
    public void AddMicrowaveEvent(MicrowaveEvent newEvent)
    {
        if (MicrowaveEvents == null) MicrowaveEvents = new List<MicrowaveEvent>();

        MicrowaveEvents.Add(newEvent);
        MicrowaveEvents.Sort();
    }
}
