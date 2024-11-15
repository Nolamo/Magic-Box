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

    private List<Microwavable> _microwavables;

    [SerializeField] private Transform _microwaveVolume;

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
        if (digit > 9) // 10+ is reset
        {
            ResetInput();
            return;
        }
        // don't accept input past 4th digit
        if (_microwaveInput.Length >= 4) return;

        _microwaveInput += digit;

        // convert the input to a 4 digit string
        _paddedInput = _microwaveInput.PadLeft(4, '0');

        // parse the input string as minutes and seconds
        int minutes = int.Parse(_paddedInput.Substring(0, 2));
        int seconds = int.Parse(_paddedInput.Substring(2, 2));

        _totalSeconds = (minutes * 60) + seconds;

        // convert the total seconds back into a string
        // this whole process seems weird, but it's necessary to cover all input cases. 
        _microwaveInput = GetDisplayText(_totalSeconds, false).TrimStart('0');

        //Debug.Log(_microwaveInput);

        OnTimerUpdate?.Invoke(GetDisplayText(_totalSeconds));
    }

    private void FixedUpdate()
    {
        if (_isMicrowaving)
        {
            Microwave();
        }
    }

    /// <summary>
    /// get display text as a string
    /// </summary>
    /// <param name="totalSeconds">the total intended cook time</param>
    /// <param name="useColon">split the MMSS with a colon, resulting in MM:SS. true by default.</param>
    /// <returns></returns>
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
        _totalSeconds -= Time.fixedDeltaTime;

        for (int i = 0; i < _microwavables.Count; i++)
        {
            _microwavables[i].Microwave(Time.fixedDeltaTime);
        }

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
        _isMicrowaving = false;
        OnMicrowaveStop?.Invoke();
    }

    public void StartMicrowaving()
    {
        if (microwaveDoor.isOpen) return;
        _isMicrowaving = true;
        OnMicrowaveStart?.Invoke();

        Collider[] objectsInMicrowave = Physics.OverlapBox(_microwaveVolume.position, _microwaveVolume.lossyScale, _microwaveVolume.rotation);

        _microwavables = new List<Microwavable>();

        for (int i = 0; i < objectsInMicrowave.Length; i++)
        {
            if (objectsInMicrowave[i].TryGetComponent(out Microwavable microwavable))
            {
                _microwavables.Add(microwavable);
                Debug.Log(objectsInMicrowave[i].gameObject.name);
            }
        }

        Debug.Log($"Count of objects in microwave:{_microwavables.Count}");
    }

    public void ToggleMicrowaving()
    {
        if (_isMicrowaving) StopMicrowaving();
        else StartMicrowaving();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(_microwaveVolume.localPosition, _microwaveVolume.localScale);
    }

    private void ResetInput()
    {
        _microwaveInput = "";
        _totalSeconds = 0;
        OnTimerUpdate?.Invoke(GetDisplayText(_totalSeconds));

    }
}
