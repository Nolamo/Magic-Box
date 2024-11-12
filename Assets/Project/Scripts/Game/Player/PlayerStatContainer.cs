using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatContainer : MonoBehaviour
{
    [field: SerializeField] public FloatStat stat { get; private set; }
    [field: SerializeField] public float maximum { get; private set; }
    [SerializeField] private float _depletionRate;
    [SerializeField] private float _depletionTimeStep = 1f;

    private float _depletionDelay;

    private void Update()
    {
        _depletionDelay -= Time.deltaTime;

        if (_depletionDelay <= 0f)
        {
            _depletionDelay += _depletionTimeStep;
            stat.SetValue(stat.value - (_depletionRate * _depletionDelay));
        }
    }
}
