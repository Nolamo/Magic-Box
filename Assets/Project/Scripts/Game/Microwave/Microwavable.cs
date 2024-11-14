using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MicrowavableProp : MonoBehaviour, IMicrowavable
{
    public float cookProgress { get; private set; }

    [SerializeField] private List<MicrowaveEvent> MicrowaveEvents;

    void Awake()
    {
        if (MicrowaveEvents == null || MicrowaveEvents.Count <= 0) 
            return;

        MicrowaveEvents.Sort();
    }

    public void Microwave(float amount)
    {
        cookProgress += amount;

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
