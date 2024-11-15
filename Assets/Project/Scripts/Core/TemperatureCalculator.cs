using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NolanCore
{
    public class NolanPhysics
    {
        [System.Serializable]
        public struct TemperatureProperties
        {
            [SerializeField] public float coolingCoefficient;
            [SerializeField] public float temperature;

            public TemperatureProperties(float coolingCoefficient = 0.1f, float temperature = 10)
            {
                this.coolingCoefficient = coolingCoefficient;
                this.temperature = temperature;
            }
        }

        public static float CalculateDeltaTemperature(float time, float ambientTemperature, float currentTemperature, float coolingCoefficient)
        {
            float desiredTemperature = Mathf.Lerp(currentTemperature, ambientTemperature, time * coolingCoefficient);
            float deltaTemperature = desiredTemperature - currentTemperature;

            return deltaTemperature;
        }

        public static float CalculateDeltaTemperature(float time, TemperatureProperties ambientProperties, TemperatureProperties properties)
        {
            float coefficient = ambientProperties.coolingCoefficient * properties.coolingCoefficient;
            float ambientTemperature = ambientProperties.temperature;
            float currentTemperature = properties.temperature;

            return CalculateDeltaTemperature(time, ambientTemperature, currentTemperature, coefficient);
        }
    }
}
