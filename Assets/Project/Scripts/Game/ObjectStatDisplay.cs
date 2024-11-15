using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectStatDisplay : MonoBehaviour
{
    private Rigidbody _rb;
    private Temperature _temperature;
    private TextMeshPro _tmp;

    [SerializeField] private bool _displayMass = true;
    [SerializeField] private bool _displayTemperature = true;

    private void Awake()
    {
        _tmp = GetComponent<TextMeshPro>();
        _rb = GetComponentInParent<Rigidbody>();
        _temperature = GetComponentInParent<Temperature>();
    }

    private void OnEnable()
    {
        if(_temperature) _temperature.OnTemperatureUpdate += UpdateText;
    }

    private void OnDisable()
    {
        if(_temperature) _temperature.OnTemperatureUpdate -= UpdateText;
    }

    void UpdateText(object sender, float temperature)
    {
        if (_tmp == null) return;
        string text = "";
        if (_rb && _displayMass) text += $"{_rb.mass}kg";
        if(_temperature && _displayTemperature) text += $"\n{Mathf.Round(temperature)}°C";
        _tmp.text = text;
    }

    void Update()
    {
        transform.up = Camera.main.transform.up;
        transform.forward = Camera.main.transform.forward;
    }
}
