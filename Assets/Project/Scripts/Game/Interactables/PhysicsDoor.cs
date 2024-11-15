using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// physics based door. Works excellently with a hinge joint

[RequireComponent(typeof(Rigidbody))]

public class PhysicsDoor : InteractableGrabbableProp, IDoor
{
    private Rigidbody _rb;

    [field: SerializeField] public bool isLocked { get; set; } = false;
    [field: SerializeField] public bool isOpen { get; set; } = false;

    [SerializeField] private float _openTorque;
    [SerializeField] private float _openDuration;
    [SerializeField] private float _openDelay;
    private float _openTime;

    [SerializeField] private Transform _directionTransform;
    [SerializeField] private bool _invertTorque;

    private const float CLOSE_THRESHOLD = 2.5f;

    private bool _inFront;

    private Vector3 _closedPosition;
    private Quaternion _closedRotation;

    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public UnityEvent OnLock;
    public UnityEvent OnUnlock;
    public UnityEvent OnOpenFailed;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) throw new System.Exception($"Door \'{gameObject.name}\' is missing a rigidbody component.");

        _closedPosition = _rb.position;
        _closedRotation = _rb.rotation;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        //Debug.Log("interacted with door!");

        if (isOpen)
        {
            Close();
        }
        else
        {
            if (interactor == null)
                return;

            Vector3 openDirection = (transform.position - interactor.transform.position).normalized;
            _inFront = Vector3.Dot(_directionTransform.forward, openDirection) > 0;
            Open();
        }
    }

    // add a force in a direction away from the player
    public void Open()
    {
        if (isLocked)
        {
            OnOpenFailed?.Invoke();
            return;
        }

        OnOpen?.Invoke();

        StartCoroutine(OpenCoroutine());
    }

    IEnumerator OpenCoroutine()
    {
        yield return new WaitForSeconds(_openDelay);

        Vector3 torque;

        if (_inFront) torque = Vector3.up * _openTorque;
        else torque = Vector3.up * -_openTorque;

        if (_invertTorque) torque *= -1;

        _openTime = Time.time + _openDuration;
        _rb.isKinematic = false;
        _rb.AddTorque(torque, ForceMode.Impulse);

        isOpen = true;

        yield break;
    }

    // add a force in the direction of the close
    public void Close()
    {
        // flip the in front bool to allow the player to close/jiggle the door
        _inFront = !_inFront;

        Vector3 torque;

        if (_inFront) torque = Vector3.up * _openTorque;
        else torque = Vector3.up * -_openTorque;

        if (_invertTorque) torque *= -1;

        _rb.AddTorque(torque * 1.5f, ForceMode.Impulse);
    }

    public void Lock()
    {
        isLocked = true;
        OnLock?.Invoke();
    }

    public void Unlock()
    {
        isLocked = false;
        OnUnlock?.Invoke();
    }

    void FixedUpdate()
    {
        // close if the door is close enough to being closed
        if(isOpen == true && Time.time > _openTime)
        {
            if(Quaternion.Angle(_rb.rotation, _closedRotation) < CLOSE_THRESHOLD)
            {
                isOpen = false;
                _rb.isKinematic = true;
                _rb.rotation = _closedRotation;
                _rb.position = _closedPosition;
                OnClose?.Invoke();
            }
        }
    }
}
