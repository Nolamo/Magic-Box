using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NolanCore;
using System;

public class TemperatureManager : SingletonMonobehaviour<TemperatureManager>
{
    [field: SerializeField] public NolanPhysics.TemperatureProperties temperatureProperties { get; set; }
    // the global temperature
    public float GlobalAmbientTemperature => temperatureProperties.temperature;

    [SerializeField] private float _timestep = 1f;
    private float _elapsedSincePreviousTick;

    public class TickEventArgs : EventArgs
    {
        public TickEventArgs (float deltaTime)
        {
            this.deltaTime = deltaTime;
        }

        public float deltaTime;
    }

    public EventHandler<TickEventArgs> OnTick;

    private void FixedUpdate()
    {
        _elapsedSincePreviousTick += Time.fixedDeltaTime;
        
        if(_elapsedSincePreviousTick >= _timestep)
        {
            TickEventArgs tickEventArgs = new TickEventArgs(_elapsedSincePreviousTick * temperatureProperties.coolingCoefficient);
            OnTick?.Invoke(this, tickEventArgs);
            _elapsedSincePreviousTick = 0;
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Destroy(Instance);
        }
    }
}
