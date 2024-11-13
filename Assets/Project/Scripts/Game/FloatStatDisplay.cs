using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatStatDisplay : StatDisplay<float>
{
    protected override void UpdateText(float value)
    {
        float roundedValue = Mathf.Round(value * 10) / 10;

        base.UpdateText(roundedValue);
    }
}
