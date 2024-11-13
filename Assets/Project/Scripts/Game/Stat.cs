using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Stat<T>
{
    [SerializeField] private T _value;
    public T value { get { return value; } }    
    public EventHandler<T> OnStatChanged;
    public GameEventAsset<T> EventAsset;

    public virtual void SetValue(T value)
    {
        this._value = value;
        OnStatChanged?.Invoke(this, value);
        EventAsset?.Invoke(value);
    }

    public T GetValue()
    {
        return value;
    }
}
