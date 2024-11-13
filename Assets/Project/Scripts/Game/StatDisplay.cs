using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StatDisplay<T> : MonoBehaviour
{
    public TextMeshProUGUI textMesh { get; private set; }

    [SerializeField] private GameEventAsset<T> _event;
    [field: SerializeField] public string text { get; private set; } = "Stat: ";
    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        _event.AddListener(UpdateText);
    }

    protected virtual void UpdateText(T value)
    {
        textMesh.text = $"{text}{value}";
    }

}
