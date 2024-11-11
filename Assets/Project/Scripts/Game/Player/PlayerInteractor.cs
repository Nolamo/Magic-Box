using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputs))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerInteractor : Interactor, IPlayerComponent
{
    // inputs
    private Rigidbody _rb;
    private PlayerInputs _playerInputs;
    private bool _inputsAreSubscribed;

    [SerializeField] private Transform _camera;

    [Header("Grabbing")]
    [SerializeField] private float _nearOffset = -0.25f;
    [SerializeField] private float _maxGrabDistance;
    [SerializeField] private float _minGrabDistance;
    [SerializeField] private float _grabStrength;
    [SerializeField] private float _grabDamper;
    private const float MASS_DAMPER_MODIFIER = 0.25f;
    private const float MIN_DAMPER = 1f;
    [SerializeField] private float _torqueSpringStrength;
    [SerializeField] private float _torqueSpringDamper;
    private float _grabDistance;

    [Header("Push / Pull")]
    [SerializeField] private float _pushPullSpeed;
    [SerializeField] private float _rotateSpeed;
    bool rotating;
    private float _scroll;
    private Vector2 _mouseLook;
    private Quaternion _offsetRotation;
    private Transform _grabRotation;


    [Header("Throwing")]
    [SerializeField] private float _throwForce = 25;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _grabRotation = new GameObject("grab rotation").transform;
        _grabRotation.SetParent(_camera);
        _grabRotation.localRotation = Quaternion.identity;
        _grabRotation.localPosition = Vector3.zero;
    }

    void OnEnable()
    {
        SubscribeInputs();
    }

    void OnDisable()
    {
        UnsubscribeInputs();
    }
    #region inputs region
    public void SubscribeInputs()
    {
        if (_playerInputs == null) _playerInputs = GetComponent<PlayerInputs>();
        if (_inputsAreSubscribed == true) return;
        _playerInputs.inputActions.Player.Grab.canceled += OnDrop;
        _playerInputs.inputActions.Player.Grab.performed += OnGrab;
        _playerInputs.inputActions.Player.Interact.performed += OnInteract;
        _playerInputs.inputActions.Player.Throw.performed += OnThrow;
        _playerInputs.inputActions.Player.PushPull.performed += OnPushPull;
        _playerInputs.inputActions.Player.Rotate.performed += OnRotate;
        _playerInputs.inputActions.Player.Rotate.canceled += OnRotate;
        _playerInputs.inputActions.Player.Look.performed += OnLook;
        _playerInputs.inputActions.Player.Look.canceled += OnLook;

    }

    public void UnsubscribeInputs()
    {
        if (_inputsAreSubscribed == false) return;
        _playerInputs.inputActions.Player.Grab.performed -= OnGrab;
        _playerInputs.inputActions.Player.Grab.canceled -= OnDrop;
        _playerInputs.inputActions.Player.Interact.performed -= OnInteract;
        _playerInputs.inputActions.Player.Throw.performed -= OnThrow;
        _playerInputs.inputActions.Player.PushPull.performed -= OnPushPull;
        _playerInputs.inputActions.Player.Rotate.performed -= OnRotate;
        _playerInputs.inputActions.Player.Rotate.canceled -= OnRotate;
        _playerInputs.inputActions.Player.Look.performed -= OnLook;
        _playerInputs.inputActions.Player.Look.canceled -= OnLook;
    }

    void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed == true)
        {
            rotating = true;
        }
        else if (context.canceled == true) 
        {
            rotating = false;
        }
        // todo: add rotation functionality
    }

    void OnLook(InputAction.CallbackContext context)
    {
        _mouseLook = context.ReadValue<Vector2>();
    }

    void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        Grab();
    }

    void OnDrop(InputAction.CallbackContext context)
    {
        if (context.canceled == false) return;
        Drop();
    }

    void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        if (grabbedObject == null) return;

        Throw();
    }

    void OnPushPull(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        _scroll = context.ReadValue<Vector2>().y;
        PushPull();
    }
    #endregion 
    public override void Grab()
    {
        base.Grab();
        if(grabbedObject == null) return;
        _grabRotation.localRotation = Quaternion.identity;
        _grabRotation.rotation = Quaternion.LookRotation(grabbedObject.transform.forward, _camera.transform.up);
    }

    void PushPull()
    {
        _grabDistance = Mathf.Clamp(_grabDistance + (_scroll * _pushPullSpeed), _minGrabDistance, _maxGrabDistance);
    }

    public void Throw()
    {
        Vector3 throwFoce = _camera.transform.forward * _throwForce;
        grabbedRB.AddForce(throwFoce, ForceMode.Impulse);

        Drop();
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        Interact();
    }

    void Update()
    {
        if (rotating)
        {
            _grabRotation.RotateAround(_grabRotation.position, _camera.transform.up, -_mouseLook.x * _rotateSpeed * Time.deltaTime);
            _grabRotation.RotateAround(_grabRotation.position, _camera.transform.right, _mouseLook.y * _rotateSpeed * Time.deltaTime);

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (grabbedObject != null)
        {
            SpringGrabbedObject();
        }
        else
        {
            CheckFocusedObject();
        }
    }

    void CheckFocusedObject()
    {

        GameObject newFocusedObject;

        // check for an object to focus on
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxGrabDistance) == false)
        {
            SetFocusedObject(null);
            return;
        }

        newFocusedObject = hit.collider.gameObject;
        
        float distance = Vector3.Distance(newFocusedObject.transform.position, _camera.position);
        _grabDistance = Mathf.Clamp(distance, _minGrabDistance, _maxGrabDistance);

        SetFocusedObject(newFocusedObject);
    }

    void ApplySpringForce(Vector3 targetPosition, Rigidbody rb, Rigidbody rb2)
    {
        // Calculate the spring force
        Vector3 rb1CenterOfMass = rb.position + rb.centerOfMass;
        Vector3 rb2CenterOfMass = rb2.position + rb2.centerOfMass;

        Vector3 displacement = targetPosition - rb1CenterOfMass;
        Vector3 springForce = displacement * _grabStrength;

        // a damper coefficient that increases damper over mass
        float damperCoefficient = rb.mass * MASS_DAMPER_MODIFIER;
        damperCoefficient = Mathf.Clamp(damperCoefficient, MIN_DAMPER, Mathf.Infinity);

        // Calculate the damping force
        Vector3 dampingForce = -rb.velocity * _grabDamper * damperCoefficient;

        // Apply the combined force to the rigidbody
        Vector3 force = springForce + dampingForce;
        rb.AddForce(force); // Use Acceleration to make it independent of mass\
    }

    void ApplySpringTorque(Quaternion targetRotation, Rigidbody rb)
    {
        // Calculate the rotation needed to get to the target
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(rb.rotation);

        // Get the shortest path for rotation (handles issues with rotations over 180 degrees)
        if (deltaRotation.w < 0)
        {
            deltaRotation.x = -deltaRotation.x;
            deltaRotation.y = -deltaRotation.y;
            deltaRotation.z = -deltaRotation.z;
            deltaRotation.w = -deltaRotation.w;
        }

        // Convert delta rotation to an angle-axis representation
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

        Vector3 dampForce = rb.angularVelocity * _torqueSpringDamper;

        // Calculate torque based on angle and axis, and apply damping to smooth rotation
        Vector3 torque = axis * (angle * Mathf.Deg2Rad * _torqueSpringStrength) - dampForce;

        // Apply torque to rotate towards the target
        rb.AddTorque(torque);
    }

    void SpringGrabbedObject()
    {
        //todo auto-drop object if it's too disconnected from the target

        Vector3 farTarget = _camera.position + (_camera.forward * _maxGrabDistance);
        Vector3 nearTarget = (_camera.position + (_camera.forward * _minGrabDistance)) + (_camera.up * _nearOffset);

        float grabAlpha = (_grabDistance - _minGrabDistance) / (_maxGrabDistance - _minGrabDistance);

        Vector3 springTarget = Vector3.Lerp(nearTarget, farTarget, grabAlpha);

        Quaternion rotationTarget = _grabRotation.rotation;

        ApplySpringTorque(rotationTarget, grabbedRB);
        // todo spring player toward heavy objects 
        ApplySpringForce(springTarget, grabbedRB, _rb);
    }
}
