using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Stat<T>
{
    [SerializeField] private T value;
    public T value { get { return value; } }    
    public EventHandler<T> OnStatChanged;

    public virtual void SetValue(T value)
    {
        this.value = value;
        OnStatChanged?.Invoke(this, value);
    }

    public T GetValue()
    {
        return value;
    }
}
