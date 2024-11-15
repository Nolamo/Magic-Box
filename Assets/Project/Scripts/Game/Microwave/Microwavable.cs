using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Temperature))]
public class Microwavable : MonoBehaviour, IMicrowavable
{
    public float cookProgress { get; private set; }

    [SerializeField] private List<MicrowaveEvent> MicrowaveEvents;

    [SerializeField, InspectorLabel("Temperature Gain"), Tooltip("Temperature increase over time while being microwaved")] private float _microwaveTemperatureGain = 10; // 10 temperature gain is enough to reach a little past 100C

    private Temperature _temperature;

    void Awake()
    {
        if (MicrowaveEvents != null || MicrowaveEvents.Count > 0)
            MicrowaveEvents.Sort();

        _temperature = GetComponent<Temperature>();
    }

    public void Microwave(float amount)
    {
        // cook the thing
        cookProgress += amount;

        if (_temperature != null)
        {
            Debug.Log($"gaining temperature: {_microwaveTemperatureGain * amount}");

            _temperature.temperature += _microwaveTemperatureGain * amount;
            _temperature.OnTemperatureUpdate?.Invoke(this, _temperature.temperature);
        }

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
