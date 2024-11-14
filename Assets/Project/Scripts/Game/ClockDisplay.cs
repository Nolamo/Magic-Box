using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class ClockDisplay : MonoBehaviour
{
    private TextMeshPro _textMesh;

    [SerializeField] private bool _useMilitaryTime;

    // Start is called before the first frame update
    void Start()
    {
        _textMesh = GetComponent<TextMeshPro>();
        DayCycleManager.Instance.OnMinuteChanged += OnMinuteChanged_UpdateText;
        UpdateText();


    }

    void OnMinuteChanged_UpdateText(object sender, TimeUpdateEventArgs args)
    {
        UpdateText();
    }

    void UpdateText()
    {
        if (_useMilitaryTime == true) _textMesh.text = DayCycleManager.Instance.GetMilitaryTime();
        else _textMesh.text = DayCycleManager.Instance.GetStandardTime();
    }
}
