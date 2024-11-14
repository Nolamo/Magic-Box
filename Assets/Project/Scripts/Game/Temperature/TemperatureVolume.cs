using NolanCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureVolume : MonoBehaviour
{
    [field: SerializeField] public int priority { get; private set; }
    [field: SerializeField] public NolanPhysics.TemperatureProperties temperatureProperties { get; private set; }

    private void OnValidate()
    {
        priority = (int)Mathf.Clamp(priority, 1, Mathf.Infinity);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Something entered {gameObject.name}");
        if (other.TryGetComponent(out Temperature temperature))
        {
            temperature.EnterVolume(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log($"Something left {gameObject.name}");
        if (other.TryGetComponent(out Temperature temperature))
        {
            temperature.ExitVolume(this);   
        }
    }
}
