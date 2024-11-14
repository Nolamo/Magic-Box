using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MicrowaveOven : MonoBehaviour
{
    private string _microwaveInput = "";
    private string _paddedInput;

    public string displayText { get; private set; }
    private float _totalSeconds;

    private bool _isMicrowaving;

    public UnityEvent<string> OnTimerUpdate;
    public UnityEvent OnMicrowaveStart;
    public UnityEvent OnMicrowaveStop;
    [Tooltip("OnMicrowaveComplete only triggers when the totalseconds have been completely depleted!")]
    public UnityEvent OnMicrowaveComplete;

    [field: SerializeField] public PhysicsDoor microwaveDoor { get; private set; }

    private void Awake()
    {
        if(microwaveDoor == null) microwaveDoor = GetComponentInChildren<PhysicsDoor>();
    }

    public void Input(int digit)
    {
        if (_isMicrowaving) return;

        // don't accept a zero input right off the bat
        if (_microwaveInput == "" && digit == 0) return;
        if (digit > 9)
        {
            _microwaveInput = "";
            _totalSeconds = 0;
            OnTimerUpdate?.Invoke(GetDisplayText(_totalSeconds));
            return;
        }
        if (_microwaveInput.Length >= 4) return;

        _microwaveInput += digit;

        _paddedInput = _microwaveInput.PadLeft(4, '0');

        int minutes = int.Parse(_paddedInput.Substring(0, 2));
        int seconds = int.Parse(_paddedInput.Substring(2, 2));

        _totalSeconds = (minutes * 60) + seconds;

        _microwaveInput = GetDisplayText(_totalSeconds, false).TrimStart('0');

        Debug.Log(_microwaveInput);

        OnTimerUpdate?.Invoke(GetDisplayText(_totalSeconds));
    }

    private void Update()
    {
        if (_isMicrowaving)
        {
            Microwave();
        }
    }

    public string GetDisplayText(float totalSeconds, bool useColon = true)
    {
        int minutes = (int)totalSeconds / 60;
        int seconds = (int)totalSeconds % 60;

        string displayText;
        if (useColon) displayText = $"{minutes:D2}:{seconds:D2}";
        else displayText = $"{minutes:D2}{seconds:D2}";

        return displayText;
    }

    private void Microwave()
    {
        _totalSeconds -= Time.deltaTime;

        string displayText = GetDisplayText(_totalSeconds);

        if (displayText != this.displayText)
        {
            this.displayText = displayText;
            OnTimerUpdate?.Invoke(displayText);
        }

        if(_totalSeconds <= 0)
        {
            StopMicrowaving();
            OnMicrowaveComplete?.Invoke();
        }
    }

    public void StopMicrowaving()
    {
        _microwaveInput = "";
        _isMicrowaving = false;
        OnMicrowaveStop?.Invoke();
    }

    public void StartMicrowaving()
    {
        if (microwaveDoor.isOpen) return;
        _microwaveInput = "";
        _isMicrowaving = true;
        OnMicrowaveStart?.Invoke();
    }

    public void ToggleMicrowaving()
    {
        if (_isMicrowaving) StopMicrowaving();
        else StartMicrowaving();
    }
}
