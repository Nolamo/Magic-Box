using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatContainer : MonoBehaviour
{
    [field: SerializeField] public FloatStat stat { get; private set; }
    [field: SerializeField] public float maximum { get; private set; } = 100f;
    // default depletion rate degrades at a rate of around 100% / 1.2 hours
    [SerializeField] private float _depletionRate = 0.02f;
    [SerializeField] private float _depletionTimeStep = 1f;

    private float _depletionDelay;

    private void Update()
    {
        _depletionDelay += Time.deltaTime;

        if (_depletionDelay >= _depletionTimeStep)
        {
            _depletionDelay -= _depletionTimeStep;
            stat.SetValue(stat.value - (_depletionRate * _depletionTimeStep));
        }
    }
}
