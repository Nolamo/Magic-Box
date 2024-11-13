using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StatDisplay<T> : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;

    [SerializeField] private GameEventAsset<T> _event;
    [SerializeField] private string _text = "Stat: ";
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textMesh.text = _text;
        _event.AddListener(UpdateText);
    }

    protected virtual void UpdateText(T value)
    {
        _textMesh.text = $"{_text}{value}";
    }

}
