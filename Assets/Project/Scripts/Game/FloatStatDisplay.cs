using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatStatDisplay : StatDisplay<float>
{
    [SerializeField] private float _maximumValue = 100;

    protected override void UpdateText(float value)
    {
        float roundedValue = Mathf.Round(value * 10) / 10;

        if(roundedValue > _maximumValue)
        {
            textMesh.text = $"{text}{_maximumValue}+";
        }
        else
        {
            base.UpdateText(roundedValue);
        }
    }
}
