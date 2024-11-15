using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    private PlayerInputs _playerInputs;
    private Camera _camera;

    private float _originalFOV;
    [SerializeField] private float _zoomedFOV;


    [SerializeField] private AnimationCurve _zoomAnimation;
    [SerializeField] private float _zoomTime;
    private float _elapsedZoom;
    private bool _isZooming;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _playerInputs = GetComponentInParent<PlayerInputs>();
        _originalFOV = _camera.fieldOfView;
    }

    private void OnEnable()
    {
        SubscribeInputs();
    }

    private void OnDisable()
    {
        UnsubscribeInputs();
    }

    void SubscribeInputs()
    {
        if (_playerInputs != null)
        {
            _playerInputs.inputActions.Player.Zoom.canceled += UnZoom;
            _playerInputs.inputActions.Player.Zoom.performed += Zoom;
        }
    }

    void UnsubscribeInputs()
    {
        if (_playerInputs != null)
        {
            _playerInputs.inputActions.Player.Zoom.canceled -= UnZoom;
            _playerInputs.inputActions.Player.Zoom.performed -= Zoom;
        }
    }

    void UnZoom(InputAction.CallbackContext context)
    {
        _isZooming = false;
    }

    void Zoom(InputAction.CallbackContext context)
    {
        _isZooming = true;
    }

    private void Update()
    {
        if (_isZooming)
            _elapsedZoom += Time.deltaTime;
        else
            _elapsedZoom -= Time.deltaTime;

        _elapsedZoom = Mathf.Clamp(_elapsedZoom, 0, _zoomTime);

        float zoomPercent = _elapsedZoom / _zoomTime;
        float zoomAlpha = _zoomAnimation.Evaluate(zoomPercent);
        _camera.fieldOfView = Mathf.Lerp(_originalFOV, _zoomedFOV, zoomAlpha);
    }
}
