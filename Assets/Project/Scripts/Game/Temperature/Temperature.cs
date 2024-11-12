using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NolanCore;
using UnityEngine.Rendering;
using System;

public class Temperature : MonoBehaviour
{
    [field: SerializeField] public NolanPhysics.TemperatureProperties _temperatureProperties;
    private List<TemperatureVolume> _activeVolumes;
    private NolanPhysics.TemperatureProperties _currentAmbientProperties;

    public EventHandler<float> OnTemperatureUpdate;

    // TO-DO: make volume-based ambient temperature compatibility

    private void Start()
    {
        _temperatureProperties.temperature = 10;
        _currentAmbientProperties = TemperatureManager.Instance.temperatureProperties;
        _activeVolumes = new List<TemperatureVolume>();
    }

    private void OnEnable()
    {
        
        TemperatureManager.Instance.OnTick += OnTick_UpdateTemperature;
    }

    private void OnDisable()
    {
        TemperatureManager.Instance.OnTick -= OnTick_UpdateTemperature;
    }

    protected virtual void OnTick_UpdateTemperature(object sender, TemperatureManager.TickEventArgs args)
    {
        TemperatureManager manager = sender as TemperatureManager;

        float deltaTemperature = NolanPhysics.CalculateDeltaTemperature(args.deltaTime, _currentAmbientProperties, _temperatureProperties);

        _temperatureProperties.temperature += deltaTemperature;

        OnTemperatureUpdate?.Invoke(this, _temperatureProperties.temperature);
    }

    public void EnterVolume(TemperatureVolume volume)
    {
        _activeVolumes.Add(volume);
        UpdateCurrentAmbience();
    }

    public void ExitVolume(TemperatureVolume volume)
    {
        _activeVolumes.Remove(volume);
        UpdateCurrentAmbience();
    }

    private void UpdateCurrentAmbience()
    {
        if (_activeVolumes.Count < 1)
        {
            _currentAmbientProperties = TemperatureManager.Instance.temperatureProperties;
            return;
        }
 

        int highestIndex = 0;
        int highestPriority = 0;

        for (int i = 0; i < _activeVolumes.Count; i++)
        {
            if (_activeVolumes[i].priority > highestPriority)
            {
                highestIndex = i;
                highestPriority = _activeVolumes[i].priority;
            }
        }
        _currentAmbientProperties = _activeVolumes[highestIndex].temperatureProperties;
    }
}
