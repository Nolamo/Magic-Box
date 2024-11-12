using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatStat : Stat<float>
{
    public override void SetValue(float value)
    {
        if (value == this.value) return;
        base.SetValue(value);
    }
}
