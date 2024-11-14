using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MicrowaveEvent : IComparable
{
    public float minProgress;
    public UnityEvent OnProgressionComplete;

    public bool TryInvoke(float progress)
    {
        if(progress >= minProgress)
        {
            OnProgressionComplete?.Invoke();
            return true;
        }
        return false;
    }

    // sort by minprogress
    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        MicrowaveEvent otherMicrowaveEvent = obj as MicrowaveEvent;
        if (otherMicrowaveEvent != null)
            return minProgress.CompareTo(otherMicrowaveEvent.minProgress);
        else 
            throw new ArgumentException("object is not a MicrowaveEvent");
    }
    public MicrowaveEvent(float minProgress)
    {
        this.minProgress = minProgress;
    }
}